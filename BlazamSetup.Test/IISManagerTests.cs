using BlazamSetup.Services; // Assuming your IISManager class is in this namespace
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.Administration;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace BlazamSetup.Tests
{
    [TestClass]
    public class IISManagerTests
    {
        private const string TestSiteName = "Blazam";
        private const string TestAppPoolName = "Blazam";
        private const string TestWwwRootDir = @"C:\inetpub\Blazam";
        private const string TestListeningAddress = "*";
        private const int TestHttpPort = 80; // Using a non-default port to avoid conflicts
        private const int TestHttpsPort = 443; // Using a non-default port

        /// <summary>
        /// This method runs once before any tests in this class.
        /// It sets up the test environment by configuring static properties,
        /// ensuring the test directory exists, and cleaning up any old test artifacts.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Clean up any remnants from a previous failed test run
            Cleanup();

            // Configure the static properties that IISManager depends on
            InstallationConfiguraion.InstallDirPath = TestWwwRootDir;
            InstallationConfiguraion.WebHostConfiguration.ListeningAddress = TestListeningAddress;
            InstallationConfiguraion.WebHostConfiguration.HttpPort = TestHttpPort;
            InstallationConfiguraion.WebHostConfiguration.HttpsPort = TestHttpsPort;
            var certificate = GetFirstAvailableCertificate();
           
            InstallationConfiguraion.WebHostConfiguration.SSLCert = certificate;

            // Ensure the physical directory for the website exists
            if (!Directory.Exists(TestWwwRootDir))
            {
                Directory.CreateDirectory(TestWwwRootDir);
            }
        }

        /// <summary>
        /// This method runs once after all tests in this class have completed.
        /// It ensures the IIS site, application pool, and test directory are removed.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            Cleanup();
        }

        /// <summary>
        /// Tests the successful creation of the IIS site and application pool without SSL.
        /// </summary>
        [TestMethod]
        public void CreateApplication()
        {
            
            
            // Act
            bool result = IISManager.CreateApplication();

            // Assert
            Assert.IsTrue(result, "IISManager.CreateApplication() should return true on success.");

            using (var serverManager = new ServerManager())
            {
                // Verify the site was created
                Site site = serverManager.Sites[TestSiteName];
                Assert.IsNotNull(site, "The IIS site 'Blazam' should be created.");

                // Verify the application pool was created and assigned
                ApplicationPool appPool = serverManager.ApplicationPools[TestAppPoolName];
                Assert.IsNotNull(appPool, "The application pool 'Blazam' should be created.");
                Assert.AreEqual(TestAppPoolName, site.Applications["/"].ApplicationPoolName, "The site should be assigned to the 'Blazam' application pool.");

                // Verify the bindings
                Assert.AreEqual(2, site.Bindings.Count, "Site should have two bindings (HTTP and HTTPS).");
                string expectedHttpBinding = $"{TestListeningAddress}:{TestHttpPort}:";
                Assert.AreEqual(expectedHttpBinding, site.Bindings[0].BindingInformation, "HTTP binding is incorrect.");
                Assert.AreEqual("http", site.Bindings[0].Protocol, "Binding protocol should be http.");


                var httpsBinding = site.Bindings.FirstOrDefault(b => b.Protocol == "https");
                Assert.IsNotNull(httpsBinding, "An HTTPS binding should have been created.");
                Assert.AreEqual(TestHttpsPort, httpsBinding.EndPoint.Port, "HTTPS binding is on the wrong port.");

                // Verify the certificate hash matches
                Assert.IsNotNull(httpsBinding.CertificateHash, "HTTPS binding should have a certificate hash.");
                CollectionAssert.AreEqual(InstallationConfiguraion.WebHostConfiguration.SSLCert.GetCertHash(), httpsBinding.CertificateHash, "The certificate hash on the binding does not match the provided certificate.");

                // Verify the physical path
                string expectedPath = Path.GetFullPath(TestWwwRootDir)+"\\";
                Assert.AreEqual(expectedPath, site.Applications["/"].VirtualDirectories["/"].PhysicalPath, "The site's physical path is incorrect.");
            }
        }

        

        /// <summary>
        /// Tests that the RemoveApplication method successfully deletes the site.
        /// </summary>
        [TestMethod]
        public void RemoveApplication_ShouldSucceed_AndRemoveSite()
        {
            // Arrange - ensure the application exists first
            IISManager.CreateApplication();


            using (var sm = new ServerManager())
            {
                Assert.IsNotNull(sm.Sites[TestSiteName], "Setup failed: Site was not created before removal test.");
            }

            // Act
            bool result = IISManager.RemoveApplication();

            // Assert
            Assert.IsTrue(result, "IISManager.RemoveApplication() should return true.");

            using (var serverManager = new ServerManager())
            {
                Site site = serverManager.Sites[TestSiteName];
                Assert.IsNull(site, "The IIS site 'Blazam' should be removed.");
            }
        }



        /// <summary>
        /// Scans the local machine's personal certificate store for a valid, non-expired
        /// certificate suitable for server authentication.
        /// </summary>
        /// <returns>An X509Certificate2 object or null if none is found.</returns>
        private static X509Certificate2 GetFirstAvailableCertificate()
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (var cert in store.Certificates)
                {
                    // Check if the certificate is valid for the current time
                    if (cert.NotAfter < DateTime.Now || cert.NotBefore > DateTime.Now) continue;

                    // Check if it has a private key
                    if (!cert.HasPrivateKey) continue;

                    // Check if its intended purpose includes Server Authentication
                    bool isServerAuth = false;
                    foreach (X509Extension extension in cert.Extensions)
                    {
                        if (extension is X509EnhancedKeyUsageExtension eku)
                        {
                            foreach (Oid oid in eku.EnhancedKeyUsages)
                            {
                                if (oid.FriendlyName == "Server Authentication")
                                {
                                    isServerAuth = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (isServerAuth)
                    {
                        return cert; // Return the first suitable certificate
                    }
                }
            }
            return null; // No suitable certificate found
        }


        /// <summary>
        /// Helper method to perform all cleanup operations.
        /// </summary>
        private static void Cleanup()
        {
            try
            {
                using (var serverManager = new ServerManager())
                {
                    // Remove Site
                    Site site = serverManager.Sites[TestSiteName];
                    if (site != null)
                    {
                        serverManager.Sites.Remove(site);
                    }

                    // Remove App Pool
                    ApplicationPool appPool = serverManager.ApplicationPools[TestAppPoolName];
                    if (appPool != null)
                    {
                        serverManager.ApplicationPools.Remove(appPool);
                    }

                    serverManager.CommitChanges();
                }

                // Remove Directory
                if (Directory.Exists(TestWwwRootDir))
                {
                    Directory.Delete(TestWwwRootDir, true);
                }
            }
            catch
            {
                // Suppress exceptions during cleanup to prevent hiding the original test failure
            }
        }
    }
}