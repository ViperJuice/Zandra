using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Zandra
{

    class SeleniumAPACSDataScraper
    {
        bool cancel = false;
        ZandraUserPreferences userPreferences;
        ObservableCollection<GetAircraftRequestResponse> Requests;
        [XmlArray("APACSRequests")]
       // private BackgroundWorker backgroundworker;
        private Window1 progressWindow;
 
        public void ScrapeAPACSRequests(ref ZandraUserPreferences userPreferences,
            ref ObservableCollection<GetAircraftRequestResponse> Requests)
        {
            this.userPreferences = userPreferences;
            this.Requests = Requests;
            progressWindow = new Window1();
            //backgroundworker = new BackgroundWorker
            //{
            //    WorkerReportsProgress = true,
            //    WorkerSupportsCancellation = true
            //};
            //backgroundworker.DoWork +=
            //    new DoWorkEventHandler(
            //        backgroundworker_DoWork);
            //backgroundworker.RunWorkerCompleted += 
            //    new RunWorkerCompletedEventHandler(
            //        backgroundworker_RunWorkerCompleted); 
            //backgroundworker.ProgressChanged +=
            //    new ProgressChangedEventHandler(
            //        backgroundworker_ProgressChanged);
            progressWindow.CancelButtonClicked += Cancel;
            progressWindow.Show();
            ScrapeRequests();
            progressWindow.Close();
            //backgroundworker.RunWorkerAsync();
        }
        private void Cancel(object sender, EventArgs e)
        {
            cancel = true;
            //backgroundworker.CancelAsync();
        }
        //private void backgroundworker_DoWork(object sender,
        //    DoWorkEventArgs e)
        //{
        //    ScrapeRequests(sender, e);
        //}
        //private void backgroundworker_RunWorkerCompleted(
        //    object sender, RunWorkerCompletedEventArgs e)
        //{
        //    progressWindow.Close();
        //    // First, handle the case where an exception was thrown.
        //    if (e.Error != null)
        //    {
        //        MessageBox.Show(e.Error.Message);
        //    }
        //    else if (e.Cancelled)
        //    {
        //        //TODO: Handle Canceled
        //        // Next, handle the case where the user canceled 
        //        // the operation.
        //        // Note that due to a race condition in 
        //        // the DoWork event handler, the Cancelled
        //        // flag may not have been set, even though
        //        // CancelAsync was called.

        //    }
        //    else
        //    {
        //        // TODO: Handle Success
        //        // Finally, handle the case where the operation 
        //        // succeeded.

        //    }
        //}
        //private void backgroundworker_ProgressChanged(object sender,
        //    ProgressChangedEventArgs e)
        //{
        //    progressWindow.ProgressBar.Value = e.ProgressPercentage;
        //    progressWindow.PercentDone.Text =  e.ProgressPercentage.ToString() + "%";
        //}

        
        private void ScrapeRequests()
        {
            //BackgroundWorker worker = sender as BackgroundWorker;
            //TODO: Add user login page capability
            //TODO:Add selenium try catch blocks 
            //Retreive APACS Data Using Selenium
            IWebDriver driver = new ChromeDriver();
            WebDriverWait wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(60));
            _ = driver.Manage().Timeouts().ImplicitWait;
            driver.Url = userPreferences.APACSLoginUrl;
            driver.Navigate().GoToUrl(userPreferences.APACSLoginUrl);

            //Ack Govt IT System
            IWebElement element = driver.FindElement(By.XPath("//*[@id='ACKNOWLEDGE_RESPONSE']"));
            element.Click();
            element = driver.FindElement(By.XPath("//*[@id='acceptButton']"));
                //Login to APACS
                element.Click();
            element = driver.FindElement(By.XPath("//*[@id='j_username']"));
            element.SendKeys(userPreferences.APACSLogin);
            element = driver.FindElement(By.XPath("//*[@id='j_password']"));
            element.SendKeys(userPreferences.APACSPassword);

            //Goto Active APACS Requests
            element = driver.FindElement(By.XPath("//*[@id='submit']"));
            element.Click();
            driver.Navigate().GoToUrl(userPreferences.APACSRequestListUrl + "1");
            List<string> requestNumbers = new List<string>();

            /*Scrape Active APACS Request IDs: The layout must be configured as follows
            Earliest Upcoming Travel Date / ID / Aircraft Call Sign(S) / Request Status / Flight Type / Itinerary ICAO*/
            int i = 1;
            //Check thath there are requests on the page otherwise stop
            while (driver.FindElements(By.XPath("//*[@id='resultList']/tbody/tr[7]")).Count > 0)
            {
                int j = 5;
                //Check that if last request on page otherwise next page
                while (driver.FindElements(By.XPath("//*[@id='resultList']/tbody/tr[" + j + "]" +
                    "/td/table/tbody/tr/td/div")).Count == 0)
                {
                    //Add request number to list
                    requestNumbers.Add(driver.FindElement(By.XPath("//*[@id='resultList']/tbody/" +
                        "tr[" + j + "]/td[2]")).Text);
                    j++;
                }
                i++;
                driver.Navigate().GoToUrl(userPreferences.APACSRequestListUrl + i);
            }

            var APACSSessionID = driver.Manage().Cookies.GetCookieNamed("JSESSIONID");
            string URL;
            //Get APACS Session ID from Selenium Controlled Chrome Browse and Pass it to WebClient
            var client = new System.Net.WebClient();
            client.Headers.Add(System.Net.HttpRequestHeader.Cookie, "JSESSIONID = " + APACSSessionID.Value);

            /*Iterate through list of APACS ID Numbers and retrieve XML requests discription using WebClient
            and add XML request to list*/
            List<string> requestsXML = new List<string>();
            i = 0;
            foreach (string requestNumber in requestNumbers)
            {
                if (cancel == true)
                {
                    //e.Cancel = true;
                    driver.Close();
                    MessageBox.Show("APACS  sync was canceled");
                }
                else
                {
                    //split work into seperatly processed chunks to allow progres bbar updatin on UI thread
                    Application.Current.Dispatcher.Invoke(new Action(delegate ()
                    {
                        URL = userPreferences.APACSRequestDownloadUrl.Replace("######", requestNumber);
                        System.Net.ServicePointManager.Expect100Continue = true;
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                        requestsXML.Add(client.DownloadString(URL));
                        i++;
                        //worker.ReportProgress(100 * i / requestNumbers.Count());
                        //ReportProgress(100 * i / requestNumbers.Count())
                        progressWindow.ProgressBar.Value = 100 * i / requestNumbers.Count();
                        progressWindow.PercentDone.Text = 100 * i / requestNumbers.Count()
                            + "% (" + i + "/" + requestNumbers.Count() + " requests)";
                    }), DispatcherPriority.Background);
                }
            }
            client.Dispose();
            client = null;
            driver.Close();
            //Deserialize retrieved APACS Requests into C# GetAircraftRequestResponse objects
            XmlSerializer serializer = new XmlSerializer(typeof(GetAircraftRequestResponse));
            foreach (string requestXML in requestsXML)
            {
                StringReader reader = new StringReader(requestXML);
                try
                {
                    Requests.Add((GetAircraftRequestResponse)serializer.Deserialize(reader));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
    }
}
