using System;
using System.Collections.Generic;
using System.DirectoryServices;

//using System.DirectoryServices.AccountManagement;
//using System.DirectoryServices.ActiveDirectory;

//Commonly Used Types /System.DirectoryServices:
//System.DirectoryServices.DirectoryEntry
//System.DirectoryServices.DirectorySearcher
//System.DirectoryServices.ActiveDirectory.ActiveDirectorySite
//System.DirectoryServices.ActiveDirectory.ApplicationPartition
//System.DirectoryServices.ActiveDirectory.DirectoryContext
//System.DirectoryServices.ActiveDirectory.DirectoryServer
//System.DirectoryServices.ActiveDirectory.Domain
//System.DirectoryServices.ActiveDirectory.DomainController

//System.DirectoryServices.AccountManagement 
//Provides uniform access and manipulation of user, 
//computer, and group security principals across the multiple principal stores: 
//Active Directory Domain Services (AD DS), 
//Active Directory Lightweight Directory Services (AD LDS), and Machine SAM (MSAM). 
//When using NuGet 3.x this package requires at least version 3.4.

namespace ConsoleAppLDAP
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("start ldap!");
            LdapQuery myDirEnt = new LdapQuery();
           
            //myDirEnt.PrintAllUsers();
            //myDirEnt.PrintCurrentDomainUserProperties();
            //myDirEnt.PrintRootProperties();
            //myDirEnt.GetAdditionalUserInfo();
            myDirEnt.GetGroups();
        }

    }

    class LdapQuery
    {
        //create an LDAP connection string
        //DirectoryEntry class is used to hold the LDAP connection string
        //DirectorySearcher class is used to perform a search against the LDAP connection
        //DirectoryEntry deBase = new DirectoryEntry("LDAP://WM2008R2ENT:389/dc=dom,dc=fr");
        //"LDAP://DC=tc-plm,DC=corp"

        public string GetCurrentDomainPath()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");
            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }
        public void GetGroups()
        {
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());
            DirectorySearcher ds = new DirectorySearcher(de);

            ds.PropertiesToLoad.Add("name");
            ds.PropertiesToLoad.Add("grouptype");
            ds.PropertiesToLoad.Add("memberof");
            ds.PropertiesToLoad.Add("member");
            ds.Filter = "(&(objectCategory=Group))";
            
            //ds.Filter = "(&(objectClass=user)(memberof:1.2.840.113556.1.4.1941:=CN=GroupOne,OU=Security Groups,OU=Groups,DC=tc-plm,DC=corp))";

            SearchResultCollection results = ds.FindAll();
            foreach (SearchResult sr in results) {
                if (sr.Properties["name"].Count > 0)
                    Console.WriteLine(sr.Properties["name"][0].ToString());

                if (sr.Properties["grouptype"].Count > 0) {
                    Console.WriteLine("  Group Type");
                    foreach (Int32 item in sr.Properties["grouptype"]) {
                        Console.WriteLine("    " + item);
                    }
                }
                if (sr.Properties["memberof"].Count > 0) {
                    Console.WriteLine("  Member of...");
                    foreach (string item in sr.Properties["memberof"]) {
                        Console.WriteLine("    " + item);
                    }
                }
                if (sr.Properties["member"].Count > 0) {
                    Console.WriteLine("  Members");
                    foreach (string item in sr.Properties["member"]) {
                        Console.WriteLine("    " + item);
                    }
                }
            }
        }

        public void GetAdditionalUserInfo()
        {
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());
            DirectorySearcher ds = BuildUserSearcher(de);
 
            ds.Filter = "(&(objectCategory=person)(objectClass=user)(name=N*))";
            //ds.Filter = "(&(objectCategory=computer))";


            SearchResultCollection results = ds.FindAll();

            foreach (SearchResult sr in results) {
                // name is always available
                Console.WriteLine(sr.Properties["name"][0].ToString());
                // distinguishedName is always available
                Console.WriteLine(sr.Properties["distinguishedName"][0].ToString());
                // if not available then empty string
                Console.WriteLine(sr.GetPropertyValue("mail"));
                Console.WriteLine(sr.GetPropertyValue("givenname"));
                Console.WriteLine(sr.GetPropertyValue("sn"));
                Console.WriteLine(sr.GetPropertyValue("userPrincipalName"));                
            }
        }
        private DirectorySearcher BuildUserSearcher(DirectoryEntry de)
        {
            DirectorySearcher ds = new DirectorySearcher(de);

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


        public void PrintAllUsers()
        {
            // connection input string -> LDAP://DC=XYZ,DC=net
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.Filter = "(&(objectCategory=User)(objectClass=person))";

            SearchResultCollection results = ds.FindAll();

            foreach (SearchResult sr in results) {
                Console.WriteLine(sr.Properties["name"][0].ToString());
            }
        }
        public void PrintRootProperties()
        {
            //Root Directory Server Agent Service Entry or RootDSE
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");
            foreach (string property in de.Properties.PropertyNames) {
                Console.WriteLine("\t{0} : {1} ", property, de.Properties[property][0]);
            }    
        }
        public void PrintCurrentDomainPath()
        {
            Console.WriteLine(GetCurrentDomainPath());
        }
        public void PrintCurrentDomainUserProperties()
        {
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.Filter = "(&(objectCategory=User)(objectClass=person))";

            SearchResultCollection results = ds.FindAll();
            foreach (SearchResult sr in results) {
                foreach (string property in sr.Properties.PropertyNames) {
                    Console.WriteLine("\t{0} : {1} ", property, sr.Properties[property][0]);
                }
            }
        }
    }
    public static class ExtensionMethodAD
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
