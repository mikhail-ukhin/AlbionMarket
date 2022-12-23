using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Models;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;

namespace AlbionMarket.Services
{
    public class AlbionItemsService
    {
        private readonly Dictionary<string, AlbionItem> albionItemsDb = new Dictionary<string, AlbionItem>();
        private readonly AlbionMarketScanerSettings _albionMarketScanerOptions;

        public AlbionItemsService(IOptions<AlbionMarketScanerSettings> albionMarketScannerOptions)
        {
            _albionMarketScanerOptions = albionMarketScannerOptions.Value;

            PrepareItemsData();
        }

        private void PrepareItemsData()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"items.json"
            );

            var content = File.ReadAllText(path);
            var originalItems = JsonSerializer.Deserialize<List<AlbionItem>>(content);

            if (originalItems != null)
            {
                foreach (var item in originalItems)
                {
                    item.ParseName();

                    albionItemsDb[item.UniqueName] = item;
                }

            }
        }

        public List<AlbionItem> GetAllItems() => albionItemsDb.Values.ToList();

        public List<AlbionItem> GetItems(string category, int? tier, int? enchant)
        {
            var expression = albionItemsDb.Values.Where(i => i.UniqueName.Contains(category));

            if (tier != null)
            {
                expression = expression.Where(i => i.Tier >= tier);
            }

            if (enchant != null)
            {
                expression = expression.Where(i => i.EnchantLevel == enchant);
            }

            return expression.ToList();
        }

        public string GetItemFriendlyName(string itemId) => GetItemInfo(itemId).LocalizedNames.EN_US;

        public AlbionItem GetItemInfo(string itemId) => albionItemsDb[itemId];

        public List<AlbionItem> GetWeaponGeneral(IEnumerable<string> itemTypeNames)
        {
            var allItems = new List<AlbionItem>();

            foreach (var item in itemTypeNames)
            {
                var albionItems = GetItems(item, _albionMarketScanerOptions.DefaultMinItemTier, null);

                if (albionItems != null)
                {
                    allItems.AddRange(albionItems);
                }
            }

            return allItems;
        }

        public List<AlbionItem> GetAllSwords()
        {
            var swordTypeNames = new List<string>
            {
                "MAIN_SWORD",
                "2H_CLAYMORE",
                "2H_DUALSWORD",
                "MAIN_SCIMITAR_MORGANA",
                "2H_CLEAVER_HELL",
                "2H_DUALSCIMITAR_UNDEAD",
                "2H_CLAYMORE_AVALON"
            };

            var swordItems = new List<AlbionItem>();

            foreach (var item in swordTypeNames)
            {
                var swords = GetItems(item, _albionMarketScanerOptions.DefaultMinItemTier, null);

                if (swords != null)
                {
                    swordItems.AddRange(swords);
                }
            }

            return swordItems;
        }

        public List<AlbionItem> GetAllArmor()
        {
            var armorTypes = new List<string>
            {
                "PLATE",
                "LEATHER",
                "CLOTH"
            };

            var itemsList = new List<string>
            {
                "HEAD",
                "SHOES",
                "ARMOR"
            };

            var armorItems = new List<AlbionItem>();

            foreach (var armor in armorTypes)
            {
                foreach (var item in itemsList)
                {
                    var armors = GetItems($"{item}_{armor}", 6, null);

                    armorItems.AddRange(armors);
                }
            }

            return armorItems;
        }

        public List<AlbionItem> GetAllAxes()
        {
            var axeTypeNames = new List<string>
            {
                "MAIN_AXE",
                "2H_AXE",
                "2H_HALBERD",
                "2H_HALBERD_MORGANA",
                "2H_SCYTHE_HELL",
                "2H_DUALAXE_KEEPER",
                "2H_AXE_AVALON"
            };

            return GetWeaponGeneral(axeTypeNames);
        }

        public List<AlbionItem> GetAllMaces()
        {
            var typeNames = new List<string>
            {
                "MAIN_MACE",
                "2H_MACE",
                "2H_FLAIL",
                "MAIN_ROCKMACE_KEEPER",
                "MAIN_MACE_HELL",
                "2H_MACE_MORGANA",
                "2H_DUALMACE_AVALON"
            };

            return GetWeaponGeneral(typeNames);
        }

        public List<AlbionItem> GetAllHammers()
        {
            var typeNames = new List<string>
            {
                "MAIN_HAMMER",
                "2H_POLEHAMMER",
                "2H_HAMMER",
                "2H_HAMMER_UNDEAD",
                "2H_DUALHAMMER_HELL",
                "2H_RAM_KEEPER",
                "2H_HAMMER_AVALON"
            };

            return GetWeaponGeneral(typeNames);
        }

        public List<AlbionItem> GetAllWarGloves()
        {
            var typeNames = new List<string>
            {
                "2H_KNUCKLES_SET1",
                "2H_KNUCKLES_SET2",
                "2H_KNUCKLES_SET3",
                "2H_KNUCKLES_KEEPER",
                "2H_KNUCKLES_HELL",
                "2H_KNUCKLES_MORGANA",
                "2H_KNUCKLES_AVALON"
            };

            return GetWeaponGeneral(typeNames);
        }

        public List<AlbionItem> GetAllWarCrossbows()
        {
            var typeNames = new List<string>
            {
                "2H_CROSSBOW",
                "2H_CROSSBOWLARGE",
                "MAIN_1HCROSSBOW",
                "2H_REPEATINGCROSSBOW_UNDEAD",
                "2H_DUALCROSSBOW_HELL",
                "2H_CROSSBOWLARGE_MORGANA",
                "2H_CROSSBOW_CANNON_AVALON"
            };

            return GetWeaponGeneral(typeNames);
        }

        public List<AlbionItem> GetAllShields()
        {
            var typeNames = new List<string>
            {
                "OFF_SHIELD",
                "OFF_TOWERSHIELD_UNDEAD",
                "OFF_SHIELD_HELL",
                "OFF_SPIKEDSHIELD_MORGANA",
                "OFF_SHIELD_AVALON"
            };

            return GetWeaponGeneral(typeNames);
        }

        public List<AlbionItem> GetAllBows()
        {
            var typeNames = new List<string>
            {
                "2H_BOW",
                "2H_WARBOW",
                "2H_LONGBOW",
                "2H_LONGBOW_UNDEAD",
                "2H_BOW_HELL",
                "2H_BOW_KEEPER",
                "2H_BOW_AVALON",
                "2H_CROSSBOW",
                "2H_CROSSBOWLARGE",
                "MAIN_1HCROSSBOW",
                "2H_DUALCROSSBOW_HELL",
                "2H_CROSSBOWLARGE_MORGANA",
                "2H_CROSSBOW_CANNON_AVALON"
            };

            return GetWeaponGeneral(typeNames);
        }
    }
}
