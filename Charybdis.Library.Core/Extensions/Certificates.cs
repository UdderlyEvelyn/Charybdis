using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Charybdis.Library.Core
{
    public static class Certificates
    {
        public static bool FIPSCompliantHashingAlgorithmsAdded = false;

        static Certificates()
        {
            try
            {
                //Put these algorithms into the config for access since the *Managed versions are not FIPS compiliant and these are.
                CryptoConfig.AddAlgorithm(typeof(SHA256CryptoServiceProvider), "SHA256CSP");
                CryptoConfig.AddAlgorithm(typeof(SHA384CryptoServiceProvider), "SHA384CSP");
                CryptoConfig.AddAlgorithm(typeof(SHA512CryptoServiceProvider), "SHA512CSP");
                FIPSCompliantHashingAlgorithmsAdded = true;
            }
            catch (MethodAccessException) //Security policies won't let us do this, so provide a method to find out.
            {
                FIPSCompliantHashingAlgorithmsAdded = false;
            }
        }

        /// <summary>
        /// Retrieves the EDIPI (sometimes referred to as "CAC ID") from a military/government CAC card certificate.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.IndexOutOfRangeException"/>
        public static string GetEDIPI(this X509Certificate2 certificate)
        {
            return certificate.Subject.Split(',')[0].Split('.').Last();
        }

        /// <summary>
        /// Retrieves the parts of the person's name from a military/government CAC card certificate via the subject line.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public static string[] GetNameParts(this X509Certificate2 certificate)
        {
            return certificate.Subject.Split(',')[0].Between("=", ".").Split('.');
        }

        /// <summary>
        /// Does the same thing that the "Clear SSL State" button in Internet Explorer's Options->Content menu does (or as close as possible).
        /// </summary>
        public static void ClearSSLState()
        {
            //This clears the SSL cache itself.
            if (!PInvoke.SCHANNEL.SslEmptyCache(IntPtr.Zero, 0))
                throw new CryptographicException(Marshal.GetLastWin32Error());
            //This triggers a new certificate prompt, otherwise it just reacquires the certificate without user intervention.
            if (!PInvoke.WININET.IncrementUrlCacheHeaderData(14, IntPtr.Zero)) 
                throw new CryptographicException(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Returns the first "Smart Card Logon" enabled certificate with no prompting (intended for use with unit tests).
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 SilentlyGetFirstLogonCertificate()
        {
            //Open certificate store.
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed | OpenFlags.OpenExistingOnly);
            //Gather certs that are used for smart card logon only.
            X509Certificate2Collection certs =
                store.Certificates
                .Find(X509FindType.FindByTimeValid, DateTime.Now, true)
                .Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, true)
                .Find(X509FindType.FindByApplicationPolicy, "Smart Card Logon", true);
            if (certs.Count == 0) //No certificates available.
                return null; //Return null to signify this.
            else
                return certs[0];
        }

        /// <summary>
        /// Prompts the user to select a certificate and returns it, optionally attempting to force PIN entry.
        /// </summary>
        /// <param name="applicationName">a string to be displayed in the selection popup</param>
        /// <param name="forcePINPrompt">if true, attempts to force PIN entry</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage] //Can't do AUT for UI methods.
        public static X509Certificate2 Prompt(string applicationName = null, bool forcePINPrompt = false)
        {
            //Open certificate store.
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed | OpenFlags.OpenExistingOnly);
            //Gather certs that are used for smart card logon only.
            X509Certificate2Collection certs =
                store.Certificates
                .Find(X509FindType.FindByTimeValid, DateTime.Now, true)
                .Find(X509FindType.FindByKeyUsage, X509KeyUsageFlags.DigitalSignature, true)
                .Find(X509FindType.FindByApplicationPolicy, "Smart Card Logon", true);
            if (certs.Count == 0) //No certificates available.
                return null; //Return null to signify this.
            //Display a native certificate selection UI and store the result.
            X509Certificate2 result = X509Certificate2UI.SelectFromCollection(certs, "FTI CAC Login", "Choose which certificate you want to log into " + (applicationName ?? "the application") + " with.", X509SelectionFlag.SingleSelection)[0];
            store.Close(); //Close the certificate store.
            if (result.HasPrivateKey) //If there's a private key..
            {
                if (!result.Verify()) //If the certificate doesn't validate..
                    throw new CryptographicException("The selected certificate failed to validate against the chain."); //Abort.
                if (forcePINPrompt) //If we're going to try to force the PIN prompt..
                {
                    SHA256CryptoServiceProvider hashProvider = new SHA256CryptoServiceProvider(); //Get a hash provider (FIPS-compliant one).
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)result.PrivateKey; //Grab the private key and convert it into a provider.
                    byte[] data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }; //DEADBEEF hex data as arbitrary (obviously-contrived) data to sign/validate.
                    try
                    {
                        byte[] signature = csp.SignData(data, hashProvider); //Sign the data with the private key through the hash provider.
                        if (csp.VerifyData(data, hashProvider, signature)) //If we can verify the signature..
                            return result; //Return the certificate, we have success!
                        else
                            return null; //Failed to access private key and/or validate a signature from it, return null to signify failure.
                    }
                    catch (ArgumentException ae) //If there's a problem with an argument..
                    {
                        if (!FIPSCompliantHashingAlgorithmsAdded) //If we haven't added the FIPS-compliant algorithms, throw an exception that is more explanatory since that's likely the culprit.
                            throw new CryptographicException("The FIPS-compliant hashing algorithms could not be added due to security policy on this machine.", ae);
                        else //Otherwise..
                            throw; //Throw the exception as normal.
                    }
                }
                else //We aren't trying to force the PIN prompt..
                    return result; //Return the certificate, success!
            }
            else
                return null; //No private key on the certificate, return null to signify failure.
        }

        /// <summary>
        /// Displays a certificate's data via the native UI functionality.
        /// </summary>
        /// <param name="certificate"></param>
        [ExcludeFromCodeCoverage] //Can't do AUT for UI methods.
        public static void Show(this X509Certificate2 certificate)
        {
            X509Certificate2UI.DisplayCertificate(certificate);
        }
    }
}
