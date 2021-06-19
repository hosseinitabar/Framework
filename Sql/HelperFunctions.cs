using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Holism.Sql
{
    public class HelperFunctions
    {
        public static Dictionary<string, int> GetCharacters(string connectionString, string query)
        {
            var table = Database.Open(connectionString).Get(query);
            var characters = new Dictionary<string, int>();
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    var text = row[column].ToString();
                    var chars = text.ToCharArray().Select(i => i.ToString()).ToList();
                    foreach (var @char in chars)
                    {
                        if (characters.ContainsKey(@char))
                        {
                            characters[@char]++;
                        }
                        else
                        {
                            characters.Add(@char, 1);
                        }
                    }
                }
            }
            return characters;
        }
    }
}
