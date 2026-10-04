using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace IDS.Tool
{
    /// <summary>
    /// Untuk CRUD (Create, Read, Update, Delete) pada form / page
    /// </summary>
    public enum PageActivity : int
    {
        none = 0,
        Insert = 1,
        Edit = 2,
        Delete = 3,
        Process = 4
    }

    /// <summary>
    /// Untuk User Group Access
    /// </summary>
    public enum GroupAccess
    {
        [Description("None")]
        None = 0,
        [Description("Read Only")]
        Read = 1,
        [Description("Create")]
        Create = 2,
        [Description("Edit")]
        Edit = 3,
        [Description("Delete")]
        Delete = 4,
        [Description("Create Edit")]
        Create_Edit = 5,
        [Description("Create Delete")]
        Create_Delete = 6,
        [Description("Edit Delete")]
        Edit_Delete = 7,
        [Description("Create Edit Delete")]
        Create_Edit_Delete = 8
    }


    #region GL Enum
    public enum GLAccountTotalDetail
    {
        AccountTotal = 1,
        AccountDetail = 0
    }

    public enum GLAccountGroup
    {
        [Description("Asset")]
        Asset = 1,
        [Description("Liabilities and Capital")]
        Liabilities_and_Capital = 2,
        [Description("Income")]
        Income = 3,
        [Description("Expense")]
        Expense = 4,
        [Description("P/L Summary")]
        PL_Summary = 5
    }

    public enum GLSpecialAccount
    {
        PL = 1,
        AR = 2,
        AP = 3,
        DL = 4,
        KS = 5,
        BN = 6,
        RE = 7,
        BY = 8
    }

    public enum GLBankStatementMatchStatus
    {
        [Description("Unmatch")]
        Unmatch = 0,
        [Description("Match")]
        Match = 1,
        [Description("All")]
        All = 2
    }

    //Add By Renaldi 4 November 2024
    public enum GLCashBank
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Cash")]
        [Description("Cash")]
        Cash = 0,
        [System.ComponentModel.DataAnnotations.Display(Name = "Bank")]
        [Description("Bank")]
        Bank = 1,
        [System.ComponentModel.DataAnnotations.Display(Name = "Others")]
        [Description("Other")]
        Others = 2
    }
    //End Add
    #endregion
}
