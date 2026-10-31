# PROJECT GUIDE — SPEKTRA POS (SPOS_RESTO)

> **File ini adalah handbook untuk agent/AI session.**
> Baca seluruh file ini SEBELUM mengerjakan apa pun. Semua yang tertulis di sini
> berdasarkan kode yang benar-benar ada di repo, bukan asumsi.

---

## 0. ATURAN KERJA WAJIB (untuk agent)

1. **JELASKAN RENCANA DULU, JANGAN LANGSUNG MENGUBAH FILE.**
   Setiap diminta membuat/mengubah sesuatu: periksa dulu kode & DB yang relevan,
   lalu sajikan rencana implementasi, dan **tunggu izin eksplisit dari user**
   sebelum menyentuh file apa pun.
2. **Jangan mengubah file/method/SP existing tanpa izin.** Jika perlu perilaku
   baru, buat method/method BARU (contoh: `GetProductListForPO` dibuat terpisah,
   tidak mengubah `GetProductListForDetail` yang dipakai modul lain).
3. **Gunakan kode existing sebagai referensi**, ikonvensi yang ada — jangan
   mengarang pola baru. File referensi terdaftar di bagian 5.
4. Setelah selesai, **jalankan build dan laporkan hasilnya** (error CS vs error
   MSB copy-lock karena VS/w3wp).
5. **Periksa dulu sebelum edit**: file bisa saja sudah diubah manual oleh user
   sejak terakhir dibaca. Selalu baca ulang bagian yang akan diedit.
6. Script SQL **diserahkan sebagai teks** untuk dijalankan user di SSMS — agent
   tidak konek langsung ke SQL Server.
7. User berkomunikasi dalam **Bahasa Indonesia**; jawab dalam Bahasa Indonesia.

---

## 1. RINGKASAN SOLUSI

- **IDE**: Visual Studio 2022, .NET 8 (`net8.0`)
- **Stack**: ASP.NET Core MVC (Razor), C#, JavaScript + jQuery, SQL Server
- **Solution**: `SPOS_RESTO.sln` (root repo)

| Project | Tipe | Isi |
|---|---|---|
| `Test1` → **SPOS.Web.UI** | ASP.NET Core MVC (net8.0) | Web UI utama: Areas, Controllers, Views, wwwroot |
| `IDS.DataAccess` → **SPOS.DataAccess** | Class Library | `SqlServer.cs`, `AbstractDataAccess.cs` — layer akses DB |
| `General` | Class Library | Model bisnis modul Catalog (`Categories`, `Product`, `Customer`, `VariantHeader`, `ProductRecipeH/D`, dll.) |
| `Transaction` | Class Library | Model transaksi: folder `Employee` (`CashierH/D`, `Shift`, `PurchaseOrderH/D`) |
| `SPOS.Inventory` | Class Library | Model inventory (folder `Stock`) |
| `IDS.Maintenance` | Class Library | Model maintenance: `User`, `UserGroup`, `GroupAccess`, `UserMenu`, `AuditTrail`, dll. |
| `IDS.GeneralTable` | Class Library | Model tabel master (`Branch`, `ItemType`, dll.) |
| `IDS.Tool` | Class Library | Helper: `GlobalVariable`, `GeneralHelper`, `SessionHelper`, `HttpContextHelper`, `Log`, dll. |
| `AppCode`, `IDS.ReportHelper` | Class Library | Pendukung; report pakai **FastReport** |

Referensi antar-project: `SPOS.Web.UI` mereferensikan semua di atas (lihat
`Test1\SPOS.Web.UI.csproj` bagian `ProjectReference`). Project `General` dan
`Transaction` SDK-style (globbing otomatis — file model baru langsung
ter-compile, tidak perlu edit csproj).

---

## 2. STRUKTUR FOLDER WEB PROJECT (`Test1`)

```
Test1/
├── Program.cs              → pipeline, session (360 menit), routing area, FastReport
├── appsettings.json        → ConnectionStrings: "Conn" (dan "ConnJF")
├── Areas/
│   ├── Catalog/            → Categories, Product, ProductRecipe, Variant
│   ├── Employee/           → Cashier, Shift, TransactionList, PurchaseOrder
│   ├── GeneralTable/       → Branch, Currency, Mainboard
│   ├── Inventory/          → StockBalance, StockTransaction
│   └── Maintenance/        → User, UserGroup, AuditTrail, MenuList, MenuGenerator, dll.
├── Controllers/            → Root: Login, Main, Menu (base), Home, Error
├── Models/                 → UserLogin, GroupAccessLevel, ErrorViewModel, dll.
├── Infrastructure/         → ViewLocationExpander (view subfolder per area)
├── Views/
│   ├── Shared/_Layout.cshtml   → layout global + helper JS global
│   └── _ViewImports.cshtml
└── wwwroot/assets/         → template frontend (Bootstrap 5) + vendor libs
```

**PENTING**: tiap Area hanya berisi `Controllers\` dan `Views\`. **Model bisnis
TIDAK di Area** — model tinggal di project class library terpisah
(`General`, `Transaction`, `IDS.Maintenance`, dll.).

Routing (`Program.cs`):
```csharp
app.MapControllerRoute("areas",   "{area:exists}/{controller=Login}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Login}/{action=Index}/{id?}");
```

---

## 3. ARSITEKTUR & POLA WAJIB

### 3.1 Akses database — TANPA ORM

Semua via ADO.NET murni (`Microsoft.Data.SqlClient`) lewat wrapper custom:

- `IDS.DataAccess\AbstractDataAccess.cs` — abstrak: `DbConnection/DbCommand/
  DbDataReader/Transaction`, `Open()`, `BeginTransaction()`, `CommitTransaction()`,
  `RollbackTransaction()`, `GetDataSet()`, `GetDataTable()`, `Close()`, `Dispose()`.
- `IDS.DataAccess\SqlServer.cs` — implementasi. Konstruktor `new SqlServer()`
  **membaca `appsettings.json` → `ConnectionStrings:Conn` sendiri** (bukan DI).
  Overload: `SqlServer(bool IsJFServer)` untuk `ConnJF`, `SqlServer(string cs)`.
- Helper: `AddParameter(name, SqlDbType, value)`, `AddParameterRange`,
  `ClearParameter()`, `ExecuteReader/ExecuteScalar/ExecuteNonQuery/GetDataTable`.

**Gotcha penting**: `ExecuteNonQuery()` **membersihkan semua parameter di
`finally`** — itulah kenapa loop insert detail di model aman memanggil
`AddParameter` berulang (parameter ter-add ulang fresh tiap iterasi).

Pola pemakaian di model (contoh nyata `General\Catalog\Categories.cs`):
- **Read**: `db.CommandText = "SELECT ..."; db.Open(); db.ExecuteReader();` →
  loop `SqlDataReader`, mapping manual per kolom via `IDS.Tool.GeneralHelper`
  (`NullToString`, `NullToInt`, `NullToInt16`, `NullToDecimal`, `NullToBool`,
  `NullToDateTime` — semua ada di `IDS.Tool\GeneralHelper.cs`).
- **Write**: stored procedure + transaksi eksplisit + handling `SqlException.Number`
  (2627 = duplicate key):
  ```csharp
  cmd.Open();
  cmd.CommandText = "InsCategories";
  cmd.CommandType = CommandType.StoredProcedure;
  cmd.AddParameter("@Type", SqlDbType.TinyInt, ExecCode);
  ...
  cmd.BeginTransaction();
  result = cmd.ExecuteNonQuery();
  cmd.CommitTransaction();
  // catch → RollbackTransaction() + throw
  ```

### 3.2 Model — pola "active record"

Model berisi data **sekaligus** method akses DB. Satu model = satu tabel (umumnya):

```csharp
public class Categories
{
    // properti + DataAnnotations ([Required], [MaxLength], dll.)
    public static List<Categories> GetData();                    // list untuk grid
    public static Categories GetDataByCategoryCode(string code); // 1 record
    public int InsUpDel(int ExecCode);                           // 1=ins, 2=upd, 3=del via SP
    public static List<SelectListItem> GetCboCategories();       // datasource dropdown
}
```

### 3.3 Controller — wajib inherit `MenuController`

Semua controller area mewarisi `IDS.Web.UI.Controllers.MenuController`
(`Test1\Controllers\MenuController.cs`):

- Konstruktor membaca session: `SESSION_USER_ID`, `SESSION_USER_GROUP_CODE`,
  `SESSION_USER_BRANCH_CODE`, `SESSION_USER_BRANCH_HO_STATUS` (key-nya di
  `IDS.Tool\GlobalVariable.cs`).
- Membangun menu sidebar dari session (`MainMenu = ParseMenuToHTML(userMenu)`) —
  **menu dari database**, bukan hardcode.
- `OnActionExecuted` → redirect ke `Main/Index` jika session habis.

Pola action standar (contoh `Areas\Catalog\Controllers\CategoriesController.cs`):

| Action | Perilaku |
|---|---|
| `Index()` | cek session → `GroupAccessLevel.GetFormGroupAccess(sUserGroup, controllerName)` → isi `ViewData["Page.Insert"/"Page.Edit"/"Page.Delete"]`, dropdown datasource, `ViewBag.UserMenu = MainMenu` → `return View()` |
| `GetData()` | endpoint POST **DataTables server-side**: baca `Request.Form["draw"/"start"/"length"/"order[0][column]"/"columns[i][name]"/"order[0][dir]"/"search[value]"]`, sorting via `System.Linq.Dynamic.Core`, filter+paging di memori, return `Json(new { draw, recordsFiltered, recordsTotal, data })` |
| `Create(int? FormAction, Model dataModal)` | cek akses → set audit field → `dataModal.InsUpDel(FormAction)` → `Json(new { msg, success, sno })` |
| `Edit(...)` / `Delete(...)` | serupa, return JSON |

Konvensi **`FormAction`: 1 = Insert, 2 = Edit, 3 = Delete** (diteruskan ke SP
sebagai `@Type`).

Hak akses: `IDS.Web.UI.Models.GroupAccessLevel` — properti `ReadAccess`,
`CreateAccess`, `EditAccess`, `DeleteAccess` (nilai -1 = tidak boleh sama sekali).

### 3.4 Konvensi database

- Nama tabel & kolom mengikuti modul; kolom audit umum:
  - Pola lama: `OperatorID`, `LastUpdate`
  - Pola baru (transaksi): `CreatedBy`, `CreatedDate`, `LastUpdateBy`, `LastUpdate`
- SP penulisan: `Ins<NamaTabel>` dengan `@Type` (1/2/3). Header-detail: SP header
  + SP detail (`@Type` 0 = delete-all detail, 1 = insert 1 baris).
- Nomor dokumen otomatis: parameter **InputOutput** di SP header
  (`@RecipeCode` INT di `InsProductRecipe`, `@POHNo` VARCHAR di `InsPurchaseOrder`),
  nilai baru dibaca di C# setelah `ExecuteNonQuery` (parameter object tetap
  terbaca walau collection di-clear).
- Tabel stok: `StockProduct` (`[date], ProdCode, Branch, <'RIB'>, BeginBalance,
  QtyIn, QtyOut, QtyAdj`) — diakses via SP `AddQtyStock(@ProdCode, @Qty, @Branch,
  @Type, @TransDate)`: Type 1=QtyOut, 2=QtyIn, 3=QtyAdj; `@TransDate` NULL → GETDATE().
- String koneksi di `Test1\appsettings.json` (`Conn`, `ConnJF`).

---

## 4. SESSION, MENU, DAN MODUL BARU

- Session key di `IDS.Tool\GlobalVariable.cs` (`SESSION_*`).
- **Sidebar menu dari DB** (`UserMenu` di session, dibangun
  `MenuController.ParseMenuToHTML`). Modul baru TIDAK muncul di sidebar sebelum
  didaftarkan → daftarkan manual via **Maintenance → MenuList / MenuGenerator**
  (isi Area, Controller), lalu beri akses di UserGroup, lalu **login ulang**
  agar session menu ter-refresh.
- Audit trail tersedia global: fungsi JS `ViewLog(key, table)` di `_Layout`
  memanggil `Maintenance/AuditTrail/Log`.

---

## 5. POLA FORM — FILE REFERENSI (baca dulu sebelum membuat form baru)

| Kebutuhan | Referensi |
|---|---|
| CRUD simple (grid + modal) | `Areas\Catalog\Controllers\CategoriesController.cs` + `Views\Categories\Index.cshtml` + `General\Catalog\Categories.cs` |
| Header-detail (modal, tab, baris dinamis) | `Areas\Catalog\Controllers\ProductRecipeController.cs` + `Views\ProductRecipe\Index.cshtml` + `General\Catalog\ProductRecipeH.cs` / `ProductRecipeD.cs` |
| Form full-page (Create view terpisah, tanpa modal) + status Proses/Cancel + stok | `Areas\Employee\Controllers\PurchaseOrderController.cs` + `Views\PurchaseOrder\Index.cshtml` + `Views\PurchaseOrder\Create.cshtml` + `Transaction\Employee\PurchaseOrderH.cs` / `PurchaseOrderD.cs` |
| Input full-page gaya POS | `Areas\Employee\Views\Cashier\Index.cshtml` |
| Grid dengan filter + date range | `Areas\Employee\Views\TransactionList\Index.cshtml` |

### Pola Purchase Order (paling baru — jadikan acuan utama form transaksi)

- **Index** = list saja: DataTables server-side + filter Branch + kolom Status
  (badge) + tombol **Add / Proses / Cancel** di `layout.topEnd.features.buttons`
  + row action (edit → navigasi ke `Create?POHNo=`, delete → AJAX).
- **Create** = form full-page untuk Add & Edit sekaligus:
  - GET `Create(string POHNo)` — POHNo kosong = Add (`FormAction=1`, model kosong,
    POHDate=now, Branch=session), POHNo diisi = Edit (`GetDataByPOHNo`, `FormAction=2`).
  - Detail rows di mode Edit **dirender server-side** (Razor foreach atas
    `Model.PODetail`, opsi product dari `ViewData["ProdCodeList"]`).
  - Save via AJAX POST → sukses → `SweetAlertSuccessWithCallback` → redirect ke
    `Edit?POHNo=` (halaman yang sama dalam mode Edit — URL membawa POHNo sehingga
    refresh aman).
  - `Edit(string POHNo)` action cukup **delegate ke `Create(POHNo)`**.
- **Proses/Cancel**: pilih row (`table.rows('.selected').data()`), validasi status
  di klien (Open→Proses, Processed→Cancel) **dan di SP** (RAISERROR), AJAX POST ke
  action `Proses`/`CancelProses`. Di controller: fetch dulu `GetDataByPOHNo` agar
  `POHDate` asli terbaca, lalu panggil method model `Proses()`/`CancelProses()`
  yang memanggil SP `ProsesPurchaseOrder`/`CancelPurchaseOrder` (transaksi +
  cursor loop detail → `EXEC AddQtyStock ...`).
- Submit detail pakai **indexing model binder**:
  ```js
  ajaxData["dataModal.POHNo"] = code;
  ajaxData[`dataModal.PODetail[${i}].ProdCode`] = item.ProdCode;
  ajaxData[`dataModal.PODetail[${i}].Qty`] = item.Qty;
  ```

---

## 6. FRONTEND STACK & HELPER JS

Semua lokal di `wwwroot/assets/` (template Bootstrap 5 + Iconify `ri-*`).

**Global via `_Layout.cshtml`**: jQuery + jquery-validation(-unobtrusive),
Bootstrap 5, **SweetAlert2** + helper global:
`SweetAlertConfirm(title, text, onConfirm)`, `SweetAlertSuccess`, `SweetAlertError`,
`SweetAlertLoading`, `SweetAlertSuccessWithCallback(title, text, callback)`,
`SweetAlertClose()`, `ViewLog(key, table)`.

**Per-halaman** (di `@section Scripts`): DataTables (+Bootstrap5) server-side,
Select2, Flatpickr (`dateFormat: "Y-m-d"` untuk input yang di-bind ke `DateTime`),
Moment.js (format tampilan `DD/MMM/YYYY`), FormValidation, Cleave-zen, Tagify, dll.

**Catatan**:
- `AddCommaDecimal` TIDAK dimuat global → definisikan lokal di view (contoh:
  `Areas\Employee\Views\Shift\Index.cshtml`).
- Select2 di dalam modal butuh `dropdownParent: $("#ModalInsert")`; di halaman
  penuh tidak perlu.
- JSON: `Program.cs` set `PropertyNamingPolicy = null` → properti C# PascalCase
  dipertahankan di JSON (`data.POHNo`, `data.PODetail`).

---

## 7. GOTCHAS (pernah terjadi — jangan diulang)

1. **`SelectList` bukan `List<SelectListItem>`** — `ViewData["X"] as List<SelectListItem>`
   menghasilkan **null** (NRE saat dipakai). Gunakan `as IEnumerable<SelectListItem>`.
2. **`DateTime.MinValue` invalid untuk SQL Server** (min = 1753-01-01). Jika JS hanya
   mengirim kode/ID, fetch dulu record dari DB untuk mendapatkan tanggal asli
   sebelum dipass ke SP (lihat action `Proses` di PurchaseOrderController).
3. **Menu baru tidak muncul** → harus didaftarkan via Maintenance → MenuList +
   akses UserGroup + login ulang.
4. **Visual Studio bisa menimpa file yang diubah agent** — jika file terbuka di VS
   dengan versi lama, Ctrl+S menimpa versi terbaru di disk. Tutup file di editor
   sebelum meminta agent mengubahnya, atau reload saat VS menawarkannya.
5. **`decimal` tanpa presisi di T-SQL = `decimal(18,0)`** → pecahan terpotong.
   Selalu tulis `decimal(18,2)`.
6. **Build "gagal" tapi bukan error kode**: `MSB3021/MSB3027` = DLL di `Test1\bin`
   dikunci `devenv.exe`/`w3wp.exe` (aplikasi berjalan). Kompilasi C# tetap
   sukses; stop aplikasi di VS lalu rebuild.
7. **T-SQL `CREATE OR ALTER PROCEDURE` harus pernyataan pertama dalam batch** —
   jalankan script SP di query window baru / pisahkan dengan `GO`; jangan
   menempel script di dalam body SP lama (menyebabkan Msg 156 & Msg 134
   duplicate DECLARE).
8. `ViewData["..."] as SelectList` yang diisi `new SelectList(...)` aman; tapi
   hati-hati jenis cast di view (lihat butir 1).
9. Modal Bootstrap bisa tertutup tidak sengaja → untuk form panjang, gunakan
   **full-page Create view** (pola Purchase Order), bukan modal.

---

## 8. CHECKLIST MENAMBAH MODUL BARU

1. **SQL** (teks untuk user): tabel header (+detail), SP `Ins<NamaTabel>`
   (`@Type` 1/2/3, nomor dokumen via parameter InputOutput bila auto),
   SP detail bila header-detail.
2. **Model** di project class library yang sesuai (`General\<Modul>`,
   `Transaction\<Modul>`, dll.): properti + `GetData(...)`,
   `GetDataBy<Key>(...)`, `InsUpDel(int)`, `GetCbo...()`; header-detail =
   `List<XDetail>` di header + loop SP detail dalam satu transaksi.
3. **Controller** di Area terkait, inherit `MenuController`: `Index`, `GetData`,
   `Create` (GET view + POST save, pisahkan dengan `[HttpGet]`/`[HttpPost]`),
   `Edit`, `Delete` (+ `Proses`/`Cancel` bila ada status). Selalu cek
   `GroupAccessLevel` dan isi `ViewData["Page.*"]`.
4. **View**: `Index.cshtml` (grid + tombol) dan — untuk form panjang —
   `Create.cshtml` full-page. Ikuti pola Purchase Order.
5. **Registrasi menu** via Maintenance → MenuList + akses UserGroup + login ulang.
6. Build + lapor hasil.

---

*Terakhir diperbarui: 31 Okt 2026 — mencakup modul Purchase Order lengkap
(CRUD full-page, Proses/Cancel dengan stok via AddQtyStock @TransDate).*
