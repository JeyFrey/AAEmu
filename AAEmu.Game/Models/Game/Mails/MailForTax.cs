using System;
using System.Collections.Generic;
using System.Text;
using AAEmu.Game.Core.Managers;
using AAEmu.Game.Core.Managers.World;
using AAEmu.Game.Core.Packets.G2C;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Models.Game.Housing;
using AAEmu.Game.Models.Game.Items;
using AAEmu.Game.Models.Game.Items.Actions;

namespace AAEmu.Game.Models.Game.Mails
{
    public class MailForTax : BaseMail
    {
        /*
         * Working example for 1.2
         * /testmail 6 .houseTax title(25) "body('Test','1606565186','1607169986','1606565186','250000','50','3','0','500000','true','1')" 0 500000
         * 
         * Values for the extra flag look as following
         * 
         */

        private House _house;

        public static string TaxSenderName = ".houseTax";
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public MailForTax(House house) : base()
        {
            _house = house;

            MailType = MailType.Billing;
            Body.RecvDate = DateTime.UtcNow ;
        }


        private static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        private static long ToUnixTime(DateTime date)
        {
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public bool UpdateTaxInfo()
        {
            // Check if owner is still valid
            var ownerName = NameManager.Instance.GetCharacterName(_house.OwnerId);
            if (ownerName == null)
                return false;
            Header.ReceiverId = _house.OwnerId;
            ReceiverName = ownerName;
            
            // Grab the zone the house is in
            var zone = ZoneManager.Instance.GetZoneByKey(_house.Position.ZoneId);
            if (zone == null)
                return false;
            var zoneGroup = ZoneManager.Instance.GetZoneGroupById(zone.GroupId);

            // Set mail title
            Title = "title(" + zone.GroupId.ToString() + ")"; // Title calls a function to call zone group name

            // Get Tax info
            if (!HousingManager.Instance.GetWeeklyTaxInfo(_house, out var totalTaxAmountDue, out var heavyTaxHouseCount, out var normalTaxHouseCount, out var hostileTaxRate))
                return false;

            var taxDueTime = _house.ProtectionEndDate.AddDays(-7);


            //testmail 6 .houseTax title(25) "body('Test','1606565186','1607169986','1606565186','250000','50','3','0','500000','true','1')" 0 500000
            Body.Text = string.Format("body('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                _house.Name,                                 // House Name
                ToUnixTime(_house.PlaceDate),                // Tax period start (this might need to be the same as tax due date)
                ToUnixTime(_house.ProtectionEndDate),        // Tax period end
                ToUnixTime(taxDueTime),                      // Tax Due Date
                _house.Template.Taxation.Tax,                // This house base tax rate
                hostileTaxRate,                              // dominion tax rate (castle tax rate ?)
                heavyTaxHouseCount,                          // number of heavy tax houses
                0,                                           // unpaid week count
                totalTaxAmountDue,                           // amount to Pay (as gold reference)
                _house.Template.HeavyTax ? "true" : "false", // is this a heavy tax building
                normalTaxHouseCount                          // number of tax-excempt houses
                );
            // In never version this has a extra field at the end, which I assume is would be the hostile tax rate

            Body.BillingAmount = totalTaxAmountDue;


            // Extra tag
            ushort extraUnknown = 0;
            Header.Extra = ((long)zoneGroup.Id << 48) + ((long)extraUnknown << 32) + ((long)_house.Id);
            Header.Status = MailStatus.Unpaid;

            return true;
        }

        /// <summary>
        /// Prepare mail
        /// </summary>
        /// <returns></returns>
        public bool Finalize()
        {
            Header.SenderId = 0;
            Header.SenderName = TaxSenderName;

            if (!UpdateTaxInfo())
                return false;

            return true;
        }

    }

}
