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
            myDirEnt.GetAllUsers();
        }
    }

    
    class LdapQuery
    {
        //create an LDAP connection string
        //The connection string for a domain named XYZ.NET looks like the following:
        //LDAP://DC=XYZ,DC=net

        //DirectoryEntry class is used to hold the LDAP connection string
        //DirectorySearcher class is used to perform a search against the LDAP connection
        public string GetCurrentDomainPath()
        {
            //DirectoryEntry deBase = new DirectoryEntry("LDAP://WM2008R2ENT:389/dc=dom,dc=fr");

            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");
            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();
        }

        //retrieve all users from your AD domain
        public void GetAllUsers()
        {
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.Filter = "(&(objectCategory=User)(objectClass=person))";

            //foreach (string property in de.Properties.PropertyNames) {
            //    Console.WriteLine("\t{0} : {1} ", property, de.Properties[property][0]);
            //}

            SearchResultCollection results = ds.FindAll();
            int i = 0;
            foreach (SearchResult sr in results) {
                foreach (string property in sr.Properties.PropertyNames) {
                    Console.WriteLine("\t{0} : {1} ", property, sr.Properties[property][0]);
                }
                i++;
                if (i == 100) break;
            }

                //foreach (SearchResult sr in results) {
                //    // Using the index zero (0) is required!
                //    // By default we get only name
                //    //Console.WriteLine(sr.Properties["name"][0].ToString());
                //    Console.WriteLine(sr.Properties.PropertyNames.ToString()  );
                //    //Console.WriteLine(sr.Properties.Count);
                //    //KeyValuePair<string, array> kvp = sr;
                //    break;
                //}
            }
        }
    
}
