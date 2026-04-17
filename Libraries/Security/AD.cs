using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using static LVS3.Enums;

namespace LVS3
{
    public static class AD
    {
        public static IDataManager DataManager;
        public static List<ADGroupData> ADGroups;

        public static void Init(IDataManager dataManager)
        {
            DataManager = dataManager;
            ADGroups = DataManager.GetAllGroups();
        }

        public static List<GroupPrincipal> GetGroups(string userName)
        {
            List<GroupPrincipal> result = new List<GroupPrincipal>();

            // establish domain context
            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userName);

            // if found - locate user groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                    }
                }
            }
            return result;
        }



        public static List<string> ADUserGroups(string userName, string domainName = null)
        {
            var result = new List<string>();
            try
            {
                if (userName.Contains('\\') || userName.Contains('/'))
                {
                    domainName = userName.Split(new char[] { '\\', '/' })[0];
                    userName = userName.Split(new char[] { '\\', '/' })[1];
                }

                using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, domainName))
                using (UserPrincipal user = UserPrincipal.FindByIdentity(domainContext, userName))
                using (var searcher = new DirectorySearcher(new DirectoryEntry("LDAP://" + domainContext.Name)))
                {
                    try
                    {
                        searcher.Filter = String.Format("(&(objectCategory=group)(member={0}))", user.DistinguishedName);
                        searcher.SearchScope = SearchScope.Subtree;
                        searcher.PropertiesToLoad.Add("cn");

                        foreach (SearchResult entry in searcher.FindAll())
                            if (entry.Properties.Contains("cn"))
                                result.Add(entry.Properties["cn"][0].ToString());
                    }
                    catch (Exception ex)
                    {
                        result.Clear();
                        string err = domainContext == null ? "domainContext=null. " : "";
                        err += user == null ? "user = null. " : "";
                        err += searcher == null ? "searcher = null. " : "";
                        err += Environment.NewLine;
                        throw new Exception("ADUserGroups() err: " + err + ex.Message);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Clear();
                throw new Exception(ex.Message);
            }
            //return result;
        }

        public static ADGroupData GetUserGroupFromRole(Roles role)
        {
            foreach (ADGroupData adgd in ADGroups)
                if (adgd.ADGroupLevel == (int)role)
                    return adgd;
            return null;
        }

        public static string GetCurrentDomainPath()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");
            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }

        private static void GetAllUsers()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new
            DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            ds.Filter = "(&(objectCategory=User)(objectClass=person))";
            results = ds.FindAll();
            foreach (SearchResult sr in results)
            {
                // Using the index zero (0) is required!
                //Debug.WriteLine(sr.Properties["name"][0].ToString());
            }
        }

        public static string GetUserInfo(string userName, string info)
        {
            if (userName == "")
                return "";
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());
            SearchResult sr;

            // Build User Searcher
            ds = buildUserSearcher(de);
            // Set the filter to look for a specific user
            ds.Filter = "(objectClass=*)";// "(&(objectCategory=User)(objectClass=person)(name=" + userName + "))";

            sr = ds.FindOne();

            if (sr != null)
                return (string)sr.GetPropertyValue(info);
            else
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    var usr = UserPrincipal.FindByIdentity(context, userName);
                    if (usr != null)
                        return usr.DisplayName;
                }
            }
            return "";
        }

        private static DirectorySearcher buildUserSearcher(DirectoryEntry de)
        {
            DirectorySearcher ds;
            ds = new DirectorySearcher(de);
            // Full Name
            ds.PropertiesToLoad.Add("name");
            // Email Address
            ds.PropertiesToLoad.Add("mail");
            // First Name
            ds.PropertiesToLoad.Add("givenname");
            // Last Name (Surname)
            ds.PropertiesToLoad.Add("sn");
            // Login Name
            ds.PropertiesToLoad.Add("userPrincipalName");
            // Distinguished Name
            ds.PropertiesToLoad.Add("distinguishedName");
            return ds;
        }

        private static void GetAllGroups()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            // Sort by name
            ds.Sort = new SortOption("name", SortDirection.Ascending);
            ds.PropertiesToLoad.Add("name");
            ds.PropertiesToLoad.Add("memberof");
            ds.PropertiesToLoad.Add("member");
            ds.Filter = "(&(objectCategory=Group))";
            results = ds.FindAll();
            foreach (SearchResult sr in results)
            {
                if (sr.Properties["name"].Count > 0)
                    Debug.WriteLine(sr.Properties["name"][0].ToString());
                if (sr.Properties["memberof"].Count > 0)
                {
                    Debug.WriteLine("  Member of...");
                    foreach (string item in sr.Properties["memberof"])
                    {
                        Debug.WriteLine("    " + item);
                    }
                }
                if (sr.Properties["member"].Count > 0)
                {
                    Debug.WriteLine("  Members");
                    foreach (string item in sr.Properties["member"])
                    {
                        Debug.WriteLine("    " + item);
                    }
                }
            }
        }

        public static bool AuthenticateUser(string domainname, string username, string password)
        {
            bool retVal = true;
            //username = "MVALVS3Account1";
            DirectoryEntry de = new DirectoryEntry("LDAP://" + domainname, username, password);
            DirectorySearcher dsearch = new DirectorySearcher(de);
            SearchResult results = null;
            try { results = dsearch.FindOne(); }
            catch (Exception ex)
            {
                retVal = false;
                string err = results == null ? "SearchResult results=null. " : "";
                err += dsearch == null ? "DirectorySearcher dsearch = null. " : "";
                err += Environment.NewLine;
                throw new Exception("AuthenticateUser() err: " + err + ex.Message);
            }
            return retVal;
        }
    }
    
    public static class ADExtensionMethods
    {
        public static string GetPropertyValue(this SearchResult sr, string propertyName)
        {
        string ret = string.Empty;

        if (sr.Properties[propertyName].Count > 0)
            ret = sr.Properties[propertyName][0].ToString();
        return ret;
        }
    }
}
