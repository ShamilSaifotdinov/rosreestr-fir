using Newtonsoft.Json;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FIR
{
    public class Programm
    {
        static Dictionary<string, string> AreaTypes = new Dictionary<string, string>()
            {
                { "001", "Площадь застройки" },
                { "002", "Общая площадь" },
                { "003", "Общая площадь без лоджии" },
                { "004", "Общая площадь с лоджией" },
                { "005", "Жилая площадь" },
                { "007", "Основная площадь" },
                { "008", "Декларированная площадь" },
                { "009", "Уточненная площадь" },
                { "010", "Фактическая площадь" },
                { "011", "Вспомогательная площадь" },
                { "012", "Площадь помещений общего пользования без лоджии" },
                { "013", "Площадь помещений общего пользования с лоджией" },
                { "014", "Технические помещения (Прочие) без лоджии" },
                { "015", "Технические помещения (Прочие) с лоджией" },
                { "020", "Застроенная площадь" },
                { "021", "Незастроенная площадь" },
                { "022", "Значение площади отсутствует" }
            };
        static void Main()
        {
            Console.Write("Текстовый файл с объектами: ");
            string src_file = Console.ReadLine();

            Console.Write("Результирующий файл: ");
            string result_file = Console.ReadLine();
            Console.WriteLine();

            if (src_file != result_file)
            {
                SearchObjs(src_file, result_file);
            }

            Console.WriteLine();
            Console.Write("Пресс эни кей...");
            Console.ReadKey();
        }
        static string Striped_id(string CID)
        {
            string[] striped = CID.Split(":");

            int[] ints = Array.ConvertAll(striped, s => int.Parse(s));

            return String.Join(':', ints);
        }
        static void SearchObjs(string src_file, string result_file)
        {
            try
            {
                File.WriteAllText(result_file, String.Empty);
                string[] lines = File.ReadAllLines(src_file);

                foreach (string line in lines)
                {
                    var objrctId = Striped_id(line);

                    string result = FIRRequest.Request(objrctId).GetAwaiter().GetResult();
                    //Console.WriteLine(result);

                    string str = line;

                    if (result == "")
                    {
                        str += " \"Не найден\"";
                    }
                    else if (result.StartsWith("Error"))
                    {
                        str += $" \"{result}\"";
                    }
                    else
                    {
                        var jsonDeserialized = JsonConvert.DeserializeObject<ObjectFIR>(result);

                        if (jsonDeserialized.parcelData.areaType != null)
                        {
                            str += " \"" + AreaTypes[jsonDeserialized.parcelData.areaType] + "\"";
                        }
                        else
                        {
                            str += " \"Тип площади не определен Росреестром\"";
                        }

                    }

                    Console.WriteLine(str);
                    File.AppendAllText(result_file, str + Environment.NewLine);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public class ObjectFIR
    {
        public string? objectId { get; set; }
        public ParcelData parcelData { get; set; }
    }

    public class ParcelData
    {
        [JsonProperty("areaType")]
        public string? areaType { get; set; }
    }
}