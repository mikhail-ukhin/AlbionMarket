namespace AlbionMarket.Core.Models
{
    public class AlbionItem
    {

        public string LocalizationNameVariable { get; set; }

        public string LocalizationDescriptionVariable { get; set; }

        public LocalazedNames LocalizedNames { get; set; }

        public string Index { get; set; }

        public string UniqueName { get; set; }

        public int Tier { get; set; }

        public int EnchantLevel { get; set; }

        public void ParseName()
        {
            if (UniqueName == null) return;

            var tiers = new List<string>
            {
                "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8"
            };

            var parts = UniqueName.Split('_');

            if (!tiers.Contains(parts[0]))
            {
                return;
            }

            Tier = parts[0] switch
            {
                "T1" => 1,
                "T2" => 2,
                "T3" => 3,
                "T4" => 4,
                "T5" => 5,
                "T6" => 6,
                "T7" => 7,
                "T8" => 8,
                _ => throw new NotImplementedException()
            };

            var secondParts = UniqueName.Split("@");

            if (secondParts.Length > 1)
            {
                EnchantLevel = secondParts[secondParts.Length - 1] switch
                {
                    "1" => 1,
                    "2" => 2,
                    "3" => 3,
                    "4" => 4
                };
            }

            else
            {
                EnchantLevel = 0;
            }


        }
    }
}
