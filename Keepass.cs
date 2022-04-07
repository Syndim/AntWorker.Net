using KeePassLib;
using KeePassLib.Keys;

namespace AntWorker.Net
{
    internal static class Keepass
    {
        public struct Entry
        {
            public string? Username { get; set; }

            public string? Password { get; set; }
        }

        private static PwDatabase? _db;

        public static void Open(string path, string password)
        {
            if (_db == null)
            {
                _db = new PwDatabase();
                var dbKey = new CompositeKey();
                dbKey.AddUserKey(new KcpPassword(password));
                _db.Open(new KeePassLib.Serialization.IOConnectionInfo
                {
                    Path = path,
                }, dbKey, null);
            }
        }

        public static Entry? GetEntry(string key)
        {
            var keys = key.Split('.').ToArray();
            return FindEntry(_db?.RootGroup, 0, keys);
        }

        private static Entry? FindEntry(PwGroup? group, int index, string[] keys)
        {
            if (group == null || index >= keys.Length)
            {
                return null;
            }

            var key = keys[index];

            if (keys.Length > 1)
            {
                foreach (var g in group.Groups)
                {
                    if (g.Name == key)
                    {
                        return FindEntry(g, index + 1, keys);
                    }
                }
            }
            else
            {
                foreach (var entry in group.Entries)
                {
                    if (entry?.Strings?.Get("Title").ReadString() == key)
                    {
                        return new Entry
                        {
                            Username = entry.Strings.Get("UserName").ReadString(),
                            Password = entry.Strings.Get("Password").ReadString(),
                        };
                    }
                }
            }

            return null;
        }
    }
}
