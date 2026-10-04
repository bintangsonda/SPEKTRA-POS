using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.GeneralTable
{
    public class CustomerSupplier
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string GLAccountNo { get; set; }

        #region Beneficiary
        public string BenName { get; set; }
        public string BenAddress1 { get; set; }
        public string BenAddress2 { get; set; }
        public string BenBank { get; set; }
        public string BenBankAccount { get; set; }
        public string BenBankAddress1 { get; set; }
        public string BenBankAddress2 { get; set; }


        #endregion


        //CUST
        //NAME
        //CONTACTPERSON
        //ADDR_1
        //ADDR_2
        //ADDR_3
        //PHONE
        //FAX
        //CUSTACC
        //BENNAME
        //BENADD1
        //BENADD2
        //BENBANK
        //BENBANKACC
        //BENBANKADD1
        //BENBANKADD2
        //ACC
        //CCY
        //TYPE
        //EntryUser
        //EntryDate
        //OperatorID
        //LastUpdate

    }
}
