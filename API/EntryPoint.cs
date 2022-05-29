﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Azure.Storage.Blobs;
using Genso.Astrology.Library;
using Genso.Astrology.Library.Compatibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace API
{
    public static class EntryPoint
    {

        //█░█ ▄▀█ █▀█ █▀▄   █▀▄ ▄▀█ ▀█▀ ▄▀█
        //█▀█ █▀█ █▀▄ █▄▀   █▄▀ █▀█ ░█░ █▀█


        //hard coded links to files stored in storage
        private const string PersonListXml = "vedastro-site-data/PersonList.xml";
        private const string MessageListXml = "vedastro-site-data/MessageList.xml";
        private const string TaskListXml = "vedastro-site-data/TaskList.xml";
        private const string VisitorLogXml = "vedastro-site-data/VisitorLog.xml";
        /// <summary>
        /// Default success message sent to caller
        /// </summary>
        private static string PassMessageXml = new XElement("Status", "Pass").ToString();




        //▄▀█ █▀█ █   █▀▀ █░█ █▄░█ █▀▀ ▀█▀ █ █▀█ █▄░█ █▀
        //█▀█ █▀▀ █   █▀░ █▄█ █░▀█ █▄▄ ░█░ █ █▄█ █░▀█ ▄█


        [FunctionName("getmatchreport")]
        public static async Task<IActionResult> Match(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Blob(PersonListXml, FileAccess.Read)] Stream personListRead,
            ILogger log)
        {
            string responseMessage;

            try
            {
                //get name of male & female
                dynamic names = await APITools.ExtractNames(req);

                //get list of all people
                var personList = new Data(personListRead);

                //generate compatibility report
                CompatibilityReport compatibilityReport = APITools.GetCompatibilityReport(names.Male, names.Female, personList);
                responseMessage = compatibilityReport.ToXml().ToString();
            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);
            //okObjectResult.ContentTypes.Add("text/html");
            return okObjectResult;
        }

        [FunctionName("addperson")]
        public static async Task<IActionResult> AddPerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get new person data out of incoming request
                //note: inside new person xml already contains user id
                var newPersonXml = APITools.ExtractDataFromRequest(incomingRequest);

                //add new person to main list
                var personListXml = APITools.AddXElementToXDocument(personListClient, newPersonXml);

                //upload modified list to storage
                await APITools.OverwriteBlobData(personListClient, personListXml);

                responseMessage = PassMessageXml;

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("addmessage")]
        public static async Task<IActionResult> AddMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(MessageListXml, FileAccess.ReadWrite)] BlobClient messageListClient)
        {
            var responseMessage = "";

            try
            {
                //get new message data out of incoming request
                //note: inside new person xml already contains user id
                var newMessageXml = APITools.ExtractDataFromRequest(incomingRequest);

                //add new message to main list
                var messageListXml = APITools.AddXElementToXDocument(messageListClient, newMessageXml);

                //upload modified list to storage
                await APITools.OverwriteBlobData(messageListClient, messageListXml);

                responseMessage = PassMessageXml;

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("addtask")]
        public static async Task<IActionResult> AddTask(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(TaskListXml, FileAccess.ReadWrite)] BlobClient taskListClient)
        {
            var responseMessage = "";

            try
            {
                //get new task data out of incoming request 
                var newTaskXml = APITools.ExtractDataFromRequest(incomingRequest);

                //add new task to main list
                var taskListXml = APITools.AddXElementToXDocument(taskListClient, newTaskXml);

                //upload modified list to storage
                await APITools.OverwriteBlobData(taskListClient, taskListXml);

                responseMessage = PassMessageXml;

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("addvisitor")]
        public static async Task<IActionResult> AddVisitor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(VisitorLogXml, FileAccess.ReadWrite)] BlobClient visitorLogClient)
        {
            var responseMessage = "";

            try
            {
                //get new visitor data out of incoming request 
                var newVisitorXml = APITools.ExtractDataFromRequest(incomingRequest);

                //add new visitor to main list
                var taskListXml = APITools.AddXElementToXDocument(visitorLogClient, newVisitorXml);

                //upload modified list to storage
                await APITools.OverwriteBlobData(visitorLogClient, taskListXml);

                responseMessage = PassMessageXml;

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("getmalelist")]
        public static async Task<IActionResult> GetMaleList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {

                //get user id
                var userId = APITools.ExtractDataFromRequest(incomingRequest).Value;

                //get person list from storage
                var personListXml = APITools.BlobClientToXml(personListClient);

                //get only male ppl into a list & matching user id
                var maleList = from person in personListXml.Root?.Elements()
                               where
                                   person.Element("Gender")?.Value == "Male" &&
                                   person.Element("UserId")?.Value == userId
                               select person;

                //send male list to caller
                responseMessage = new XElement("Root", maleList).ToString();

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        /// <summary>
        /// Gets all the unique visitors to the site
        /// </summary>
        [FunctionName("getvisitorlist")]
        public static async Task<IActionResult> GetVisitorList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(VisitorLogXml, FileAccess.ReadWrite)] BlobClient visitorLogClient)
        {
            var responseMessage = "";

            try
            {

                //get user id
                var userId = APITools.ExtractDataFromRequest(incomingRequest).Value;

                //get visitor log from storage
                var visitorLogXml = APITools.BlobClientToXml(visitorLogClient);

                //get all unique visitor elements only
                var uniqueVisitorList = from visitorXml in visitorLogXml.Root?.Elements()
                                        where
                                            //note: location tag only exists for new visitor log,
                                            //so use that to get unique list
                                            visitorXml.Element("Location") != null
                                        select visitorXml;

                //send list to caller
                responseMessage = new XElement("Root", uniqueVisitorList).ToString();

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("getfemalelist")]
        public static async Task<IActionResult> GetFemaleList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get user id
                var userId = APITools.ExtractDataFromRequest(incomingRequest).Value;

                //get person list from storage
                var personListXml = APITools.BlobClientToXml(personListClient);

                //get only female ppl into a list
                var femaleList = from person in personListXml.Root?.Elements()
                                 where
                                     person.Element("Gender")?.Value == "Female"
                                     &&
                                     person.Element("UserId")?.Value == userId
                                 select person;

                //send female list to caller
                responseMessage = new XElement("Root", femaleList).ToString();

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        /// <summary>
        /// Gets person all details from only hash
        /// </summary>
        [FunctionName("getperson")]
        public static async Task<IActionResult> GetPerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get hash that will be used find the person
                var requestData = APITools.ExtractDataFromRequest(incomingRequest);
                var originalHash = int.Parse(requestData.Value);

                //get the person record by hash
                var personListXml = APITools.BlobClientToXml(personListClient);
                var foundPerson = APITools.FindPersonByHash(personListXml, originalHash);

                //send person to caller
                responseMessage = new XElement("Root", foundPerson).ToString();

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        /// <summary>
        /// Generates a new SVG dasa report given a person hash
        /// </summary>
        [FunctionName("getpersondasareportOLD")]
        public static async Task<IActionResult> GetPersonDasaReportOLD(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get dasa report for sending
                var dasaReportSvg = await GetDasaReportSvgForIncomingRequest(incomingRequest);

                //send image back to caller
                var x = streamToByteArray(dasaReportSvg);
                return new FileContentResult(x, "image/svg+xml");

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;

            //

            async Task<Stream> GetDasaReportSvgForIncomingRequest(HttpRequestMessage req)
            {
                //get hash that will be used find the person
                var requestData = APITools.ExtractDataFromRequest(req);
                var originalHash = int.Parse(requestData.Value);

                //get the person instance by hash
                var personListXml = APITools.BlobClientToXml(personListClient);
                var foundPersonXml = APITools.FindPersonByHash(personListXml, originalHash);
                var foundPerson = Person.FromXml(foundPersonXml);

                //from person get svg report
                var dasaReportSvgString = await GetDasaReportSvgFromApi(foundPerson);

                //convert svg string to stream for sending
                //todo check if using really needed here
                var stream = GenerateStreamFromString(dasaReportSvgString);

                return stream;
            }

        }

        /// <summary>
        /// Generates a new SVG dasa report given a person hash
        /// Exp call:
        /// <Root>
        //      <PersonHash>374117709</PersonHash>
        //      <StartTime>
        //          <Time>
        //              <StdTime>00:00 01/01/1994 +08:00</StdTime>
        //              <Location>
        //                  <Name>Teluk Intan</Name>
        //                  <Longitude>101.0206</Longitude>
        //                  <Latitude>4.0224</Latitude>
        //              </Location>
        //          </Time>
        //  </StartTime>
        //  <EndTime>
        //      <Time>
        //          <StdTime>11:59 31/12/2024 +08:00</StdTime>
        //          <Location>
        //              <Name>Teluk Intan</Name>
        //              <Longitude>101.0206</Longitude>
        //              <Latitude>4.0224</Latitude>
        //          </Location>
        //      </Time>
        //  </EndTime>
        //  <DaysPerPixel>11</DaysPerPixel>
        //</Root>
        /// </summary>
        [FunctionName("getpersondasareport")]
        public static async Task<IActionResult> GetPersonDasaReport(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get dasa report for sending
                var dasaReportSvg = await GetDasaReportSvgForIncomingRequest(incomingRequest);

                //send image back to caller
                var x = streamToByteArray(dasaReportSvg);
                return new FileContentResult(x, "image/svg+xml");

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;

            //

            async Task<Stream> GetDasaReportSvgForIncomingRequest(HttpRequestMessage req)
            {
                //get all the data needed out of the incoming request
                var rootXml = APITools.ExtractDataFromRequest(req);
                var personHash = int.Parse(rootXml.Element("PersonHash").Value);
                var startTimeXml = rootXml.Element("StartTime").Elements().First();
                var startTime = Time.FromXml(startTimeXml);
                var endTimeXml = rootXml.Element("EndTime").Elements().First();
                var endTime = Time.FromXml(endTimeXml);
                var daysPerPixel = double.Parse(rootXml.Element("DaysPerPixel").Value);


                //get the person instance by hash
                var personListXml = APITools.BlobClientToXml(personListClient);
                var foundPersonXml = APITools.FindPersonByHash(personListXml, personHash);
                var foundPerson = Person.FromXml(foundPersonXml);

                //from person get svg report
                //time range to generate is from beginning of year to end
                var timeZone = foundPerson.BirthTimeZone;
                var geoLocation = foundPerson.GetBirthLocation();
                //Time startTime = new Time($"00:00 01/01/2022 {timeZone}", geoLocation);
                //Time endTime = new Time($"00:00 31/12/2022 {timeZone}", geoLocation);
                var dasaReportSvgString = await GetDasaReportSvgFromApi2(foundPerson, startTime, endTime, daysPerPixel);


                //convert svg string to stream for sending
                //todo check if using really needed here
                var stream = GenerateStreamFromString(dasaReportSvgString);

                return stream;
            }

        }

        /// <summary>
        /// Updates a person's record, uses hash to identify person to overwrite
        /// </summary>
        [FunctionName("updateperson")]
        public static async Task<IActionResult> UpdatePerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get unedited hash & updated person details from incoming request
                var requestData = APITools.ExtractDataFromRequest(incomingRequest);
                var originalHash = int.Parse(requestData?.Element("PersonHash").Value);
                var updatedPersonXml = requestData?.Element("Person");

                //get the person record that needs to be updated
                var personListXml = APITools.BlobClientToXml(personListClient);
                var personToUpdate = APITools.FindPersonByHash(personListXml, originalHash);

                //delete the previous person record,
                //and insert updated record in the same place
                personToUpdate.ReplaceWith(updatedPersonXml);

                //upload modified list to storage
                await APITools.OverwriteBlobData(personListClient, personListXml);

                responseMessage = PassMessageXml;

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        /// <summary>
        /// Deletes a person's record, uses hash to identify person
        /// Note : user id is not checked here because Person hash
        /// can't even be generated by client side if you don't have access.
        /// Theoretically anybody who gets the hash of the person,
        /// can delete the record by calling this API
        /// </summary>
        [FunctionName("deleteperson")]
        public static async Task<IActionResult> DeletePerson(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get unedited hash & updated person details from incoming request
                var requestData = APITools.ExtractDataFromRequest(incomingRequest);
                var originalHash = int.Parse(requestData.Value);

                //get the person record that needs to be deleted
                var personListXml = APITools.BlobClientToXml(personListClient);
                var personToDelete = APITools.FindPersonByHash(personListXml, originalHash);

                //delete the person record,
                personToDelete.Remove();

                //upload modified list to storage
                await APITools.OverwriteBlobData(personListClient, personListXml);

                responseMessage = PassMessageXml;

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        /// <summary>
        /// Deletes a person's record, uses hash to identify person
        /// Note : user id is not checked here because Person hash
        /// can't even be generated by client side if you don't have access.
        /// Theoretically anybody who gets the hash of the person,
        /// can delete the record by calling this API
        /// </summary>
        [FunctionName("deletevisitor")]
        public static async Task<IActionResult> DeleteVisitor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(VisitorLogXml, FileAccess.ReadWrite)] BlobClient visitorLogClient)
        {
            var responseMessage = "";

            try
            {
                //get unedited hash & updated person details from incoming request
                var requestData = APITools.ExtractDataFromRequest(incomingRequest);
                var visitorId = requestData.Value;

                //get the person record that needs to be deleted
                var visitorLogXml = APITools.BlobClientToXml(visitorLogClient);
                var visitorToDelete = APITools.FindVisitorById(visitorLogXml, visitorId);

                //delete the person record,
                visitorToDelete.Remove();

                //upload modified list to storage
                await APITools.OverwriteBlobData(visitorLogClient, visitorLogXml);

                responseMessage = PassMessageXml;

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("getpersonlist")]
        public static async Task<IActionResult> GetPersonList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(PersonListXml, FileAccess.ReadWrite)] BlobClient personListClient)
        {
            var responseMessage = "";

            try
            {
                //get user id
                var userId = APITools.ExtractDataFromRequest(incomingRequest).Value;

                //get all person list from storage
                var personListXml = APITools.BlobClientToXml(personListClient);

                //filter out person by user id
                var filteredList = APITools.FindPersonByUserId(personListXml, userId);

                //send filtered list to caller
                responseMessage = new XElement("Root", filteredList).ToString();

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("gettasklist")]
        public static async Task<IActionResult> GetTaskList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob(TaskListXml, FileAccess.ReadWrite)] BlobClient taskListClient)
        {
            var responseMessage = "";

            try
            {
                //get task list from storage
                var taskListXml = APITools.BlobClientToXml(taskListClient);

                //send task list to caller
                responseMessage = taskListXml.ToString();

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }

        [FunctionName("getevents")]
        public static async Task<IActionResult> GetEvents(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage incomingRequest,
            [Blob("vedastro-site-data/EventDataList.xml", FileAccess.ReadWrite)] BlobClient eventDataListClient)
        {
            var responseMessage = "";

            try
            {

                //get person list from storage
                var eventDataListXml = APITools.BlobClientToXml(eventDataListClient);

                //get data needed to generate events
                var requestData = APITools.ExtractDataFromRequest(incomingRequest);

                //parse it
                var person = Person.FromXml(requestData.Element("Person"));
                var startTime = Time.FromXml(requestData.Element("StartTime").Element("Time"));
                var endTime = Time.FromXml(requestData.Element("EndTime").Element("Time"));
                var location = GeoLocation.FromXml(requestData.Element("Location"));
                var tag = Tools.XmlToAnyType<EventTag>(requestData.Element(typeof(EventTag).FullName));
                var precision = Tools.XmlToAnyType<double>(requestData.Element(typeof(double).FullName));

                //calculate events from the data received
                var events = CalculateEvents(startTime, endTime, location, person, tag, precision, eventDataListXml);

                //convert events to XML for sending
                var rootXml = new XElement("Root");
                foreach (var _event in events)
                {
                    rootXml.Add(_event.ToXml());
                }

                responseMessage = rootXml.ToString();

            }
            catch (Exception e)
            {
                //format error nicely to show user
                responseMessage = APITools.FormatErrorReply(e);
            }


            var okObjectResult = new OkObjectResult(responseMessage);

            return okObjectResult;
        }





        //█▀█ █▀█ █ █░█ ▄▀█ ▀█▀ █▀▀   █▀▄▀█ █▀▀ ▀█▀ █░█ █▀█ █▀▄ █▀
        //█▀▀ █▀▄ █ ▀▄▀ █▀█ ░█░ ██▄   █░▀░█ ██▄ ░█░ █▀█ █▄█ █▄▀ ▄█


        public static List<Event> CalculateEvents(Time startTime, Time endTime, GeoLocation location, Person person, EventTag tag, double precision, XDocument dataEventdatalistXml)
        {

            //parse each raw event data in list
            var eventDataList = new List<EventData>();
            foreach (var eventData in dataEventdatalistXml.Root.Elements())
            {
                //add it to the return list
                eventDataList.Add(EventData.ToXml(eventData));
            }

            //get all event data/types which has the inputed tag (FILTER)
            var eventDataListFiltered = DatabaseManager.GetEventDataListByTag(tag, eventDataList);

            //TODO event generation time logging enable when can
            ////debug to measure event calculation time
            //var watch = Stopwatch.StartNew();

            //start calculating events
            var eventList = EventManager.GetEventsInTimePeriod(startTime.GetStdDateTimeOffset(), endTime.GetStdDateTimeOffset(), location, person, precision, eventDataListFiltered);

            //watch.Stop();
            //LogManager.Debug($"Events computed in: { watch.Elapsed.TotalSeconds}s");

            return eventList;
        }

        /// <summary>
        /// The massive method that generates every inch of the dasa svg report
        /// </summary>
        private static async Task<string> GetDasaReportSvgFromApi(Person inputPerson)
        {

            //px width & height of each slice of time
            //used when generating dasa rows
            //note: changes needed only here
            int _widthPerSlice = 1;
            int _heightPerSlice = 50;
            //the height for all lines, cursor, now & life events line
            var lineHeight = 170;


            // One precision value for generating all dasa components,
            // because misalignment occurs if use different precision
            double _eventsPrecision = Tools.DaysToHours(14);

            double _timeSlicePrecision = _eventsPrecision;


            //prep data
            var startTime = inputPerson.BirthTime; //start time is birth time
            var endTime = startTime.AddYears(120); //end time is 120 years from birth (dasa cycle)

            //use the inputed data to get events from API
            //note: below methods access the data internally
            var dasaEventList = await GetDasaEvents(_eventsPrecision, startTime, endTime, inputPerson);
            var bhuktiEventList = await GetBhuktiEvents(_eventsPrecision, startTime, endTime, inputPerson);
            var antaramEventList = await GetAntaramEvents(_eventsPrecision, startTime, endTime, inputPerson);

            //generate rows and pump them final svg string
            var dasaSvgWidth = 0; //will be filled when calling row generator
            var compiledRow = "";

            //generate time slice only once for all rows
            var timeSlices = GetTimeSlices();

            //save a copy of the number of time slices used to calculate the svg total width later
            dasaSvgWidth = timeSlices.Count;

            //note: y axis positions are manually set
            compiledRow += GenerateYearRowSvg(dasaEventList, timeSlices, _eventsPrecision, 0);
            compiledRow += GenerateRowSvg(dasaEventList, timeSlices, _eventsPrecision, 12);
            compiledRow += GenerateRowSvg(bhuktiEventList, timeSlices, _eventsPrecision, 65);
            compiledRow += GenerateRowSvg(antaramEventList, timeSlices, _eventsPrecision, 118);

            //add in the cursor line
            compiledRow += $"<rect id=\"CursorLine\" width=\"2\" height=\"{lineHeight}\" style=\"fill:#000000;\" x=\"0\" y=\"0\" />";

            //get now line position
            var nowLinePosition = GetLinePosition(timeSlices, DateTimeOffset.Now);
            compiledRow += $"<rect id=\"NowVerticalLine\" width=\"2\" height=\"{lineHeight}\" style=\"fill:blue;\" x=\"0\" y=\"0\" transform=\"matrix(1, 0, 0, 1, {nowLinePosition}, 0)\" />";

            //wait!, add in life events also
            compiledRow += GetLifeEventLinesSvg(inputPerson);

            //compile the final svg
            var finalSvg = WrapSvgElements(compiledRow, dasaSvgWidth, (_heightPerSlice * 3) + 60); //little wiggle room

            return finalSvg;



            //█░░ █▀█ █▀▀ ▄▀█ █░░   █▀▀ █░█ █▄░█ █▀▀ ▀█▀ █ █▀█ █▄░█ █▀
            //█▄▄ █▄█ █▄▄ █▀█ █▄▄   █▀░ █▄█ █░▀█ █▄▄ ░█░ █ █▄█ █░▀█ ▄█


            //gets person's life events as lines for the dasa chart
            string GetLifeEventLinesSvg(Person person)
            {
                var compiledLines = "";

                foreach (var lifeEvent in person.LifeEventList)
                {

                    //get start time of life event and find the position of it in slices (same as now line)
                    //so that this life event line can be placed exactly on the report where it happened
                    var startTime = DateTimeOffset.ParseExact(lifeEvent.StartTime, Time.GetDateTimeFormat(), null);
                    var position = GetLinePosition(timeSlices, startTime);

                    //note: this is the icon below the life event line to magnify it
                    var iconSvg = @"
                                <g style="""" transform=""matrix(1.976054, 0, 0, 2.056383, -0.672014, -84.991478)"">
                                <rect style=""fill: rgb(255, 242, 0); stroke: rgb(0, 0, 0); stroke-width: 0.495978px;"" x=""-5.177"" y=""124"" width=""12.477"" height=""9.941"" rx=""2.479"" ry=""2.479""></rect>
                                <path d=""M 2.7 129.226 L 1.478 129.226 C 1.254 129.226 1.071 129.403 1.071 129.618 L 1.071 130.793 C 1.071 131.009 1.254 131.185 1.478 131.185 L 2.7 131.185 C 2.923 131.185 3.106 131.009 3.106 130.793 L 3.106 129.618 C 3.106 129.403 2.923 129.226 2.7 129.226 Z M 2.7 125.311 L 2.7 125.703 L -0.557 125.703 L -0.557 125.311 C -0.557 125.095 -0.74 124.92 -0.965 124.92 C -1.189 124.92 -1.372 125.095 -1.372 125.311 L -1.372 125.703 L -1.778 125.703 C -2.231 125.703 -2.589 126.055 -2.589 126.486 L -2.593 131.968 C -2.593 132.401 -2.228 132.751 -1.778 132.751 L 3.921 132.751 C 4.369 132.751 4.734 132.399 4.734 131.968 L 4.734 126.486 C 4.734 126.055 4.369 125.703 3.921 125.703 L 3.513 125.703 L 3.513 125.311 C 3.513 125.095 3.33 124.92 3.106 124.92 C 2.883 124.92 2.7 125.095 2.7 125.311 Z M 3.513 131.968 L -1.372 131.968 C -1.595 131.968 -1.778 131.792 -1.778 131.576 L -1.778 127.661 L 3.921 127.661 L 3.921 131.576 C 3.921 131.792 3.737 131.968 3.513 131.968 Z"" style=""""></path>
                                </g>
                                 ";

                    compiledLines += $"<g" +
                                     $" eventName=\"{lifeEvent.Name}\" " +
                                     $" age=\"{inputPerson.GetAge(startTime.Year)}\" " +
                                     $" stdTime=\"{startTime:dd/MM/yyyy}\" " + //show only date
                                     $" transform=\"matrix(1, 0, 0, 1, {position}, 0)\"" +
                                    $" x=\"0\" y=\"0\" >" +
                                    $"<rect" +
                                    $" width=\"2\"" +
                                    $" height=\"{lineHeight}\"" +
                                    $" style=\"fill:#fff200;\"" +
                                    //$" x=\"0\"" +
                                    //$" y=\"0\" " +
                                    $" />" + iconSvg +
                                    "</g>";

                }


                return compiledLines;
            }

            //gets line position given a date
            //finds most closest time slice, else return 0 means none found
            int GetLinePosition(List<Time> timeSliceList, DateTimeOffset inputTime)
            {
                var nowYear = inputTime.Year;
                var nowMonth = inputTime.Month;

                //go through the list and find where the slice is closest to now
                var slicePosition = 0;
                foreach (var time in timeSliceList)
                {

                    //if same year and same month then send this slice position
                    //as the correct one
                    var sameYear = time.GetStdYear() == nowYear;
                    var sameMonth = time.GetStdMonth() == nowMonth;
                    if (sameMonth && sameYear)
                    {
                        return slicePosition;
                    }

                    //move to next slice position
                    slicePosition++;
                }

                //if control reaches here then now time not found in time slices
                //this is possible when viewing old charts as such set now line to 0
                return 0;

            }


            string GenerateRowSvg(List<Event> eventList, List<Time> timeSlices, double precisionHours, int yAxis)
            {
                //generate the row for each time slice
                var rowHtml = "";
                var horizontalPosition = 0; //distance from left
                var prevEventName = EventName.EmptyEvent;
                foreach (var slice in timeSlices)
                {
                    //get event that occurred at this time slice
                    //if more than 1 event raise alarm
                    var foundEventList = eventList.FindAll(tempEvent => tempEvent.IsOccurredAtTime(slice));
                    if (foundEventList.Count > 1) throw new Exception("Only 1 event in 1 time slice!");
                    var foundEvent = foundEventList[0];

                    //if current event is different than event has changed, so draw a black line
                    var isNewEvent = prevEventName != foundEvent.Name;
                    var color = isNewEvent ? "black" : GetEventColor(foundEvent?.Nature);
                    prevEventName = foundEvent.Name;

                    //generate and add to row
                    //the hard coded attribute names used here are used in App.js
                    var rect = $"<rect " +
                               $"eventName=\"{foundEvent?.FormattedName}\" " +
                               $"age=\"{inputPerson.GetAge(slice)}\" " +
                               $"stdTime=\"{slice.GetStdDateTimeOffset():dd/MM/yyyy}\" " + //show only date
                               $"x=\"{horizontalPosition}\" " +
                               $"width=\"{_widthPerSlice}\" " +
                               $"height=\"{_heightPerSlice}\" " +
                               $"fill=\"{color}\" />";

                    //set position for next element
                    horizontalPosition += _widthPerSlice;

                    rowHtml += rect;

                }

                //wrap all the rects inside a svg so they can me moved together
                //svg tag here acts as group, svg nesting
                rowHtml = $"<svg y=\"{yAxis}\" >{rowHtml}</svg>";

                return rowHtml;
            }


            string GenerateYearRowSvg(List<Event> eventList, List<Time> timeSlices, double precisionHours, int yAxis)
            {


                //generate the row for each time slice
                var rowHtml = "";
                var previousYear = 0;
                var yearBoxWidthCount = 0;
                int rectWidth = 0;
                int newX = 0;
                foreach (var slice in timeSlices)
                {

                    //only generate new year box when year changes
                    var yearChanged = previousYear != slice.GetStdYear();

                    //if year changed
                    if (yearChanged)
                    {
                        //and it is in the beginning
                        if (previousYear == 0)
                        {
                            yearBoxWidthCount = 0; //reset width
                        }
                        else
                        {
                            //generate previous year data first before resetting
                            newX += rectWidth; //use previous rect width to position this
                            rectWidth = yearBoxWidthCount * _widthPerSlice; //calculate new rect width

                            var rect = $"<g x=\"{newX}\" y=\"{20}\" transform=\"matrix(1, 0, 0, 1, {newX}, 0)\">" +
                                                $"<rect " +
                                                    $"fill=\"#0d6efd\" x=\"0\" y=\"0\" width=\"{rectWidth}\" height=\"{11}\" rx=\"0\" ry=\"0\"" + $"style=\"paint-order: stroke; stroke: rgb(255, 255, 255); stroke-opacity: 1; stroke-linejoin: round;\"/>" +
                                                    $"<text x=\"0\" y=\"18.034\" fill=\"white\" " +
                                                        $"style=\"fill: rgb(255, 255, 255); font-size: 10.2278px; font-weight: 700; line-height: 36.3655px; white-space: pre;\"" +
                                                        $"transform=\"matrix(0.966483, 0, 0, 0.879956, 2, -6.779947)\"" +
                                                        $"x=\"0\" y=\"18.034\" bx:origin=\"0.511627 0.5\">" +
                                                        $"{previousYear}" + //previous year generate at begin of new year
                                                    $"</text>" +
                                             $"</g>";


                            //add to final return
                            rowHtml += rect;

                            //reset width
                            yearBoxWidthCount = 0;

                        }
                    }
                    //year same as before
                    else
                    {
                        //update width only, position is same
                        //as when created the year box
                        //yearBoxWidthCount *= _widthPerSlice;

                    }

                    //update previous year for next slice
                    previousYear = slice.GetStdYear();

                    yearBoxWidthCount++;


                }

                //wrap all the rects inside a svg so they can me moved together
                //svg tag here acts as group, svg nesting
                //rowHtml = $"<svg y=\"{yAxis}\" >{rowHtml}</svg>";

                return rowHtml;
            }


            // Get dasa color based on nature & number of events
            string GetEventColor(EventNature? eventNature)
            {
                var colorId = "gray";

                if (eventNature == null) { return colorId; }

                //set color id based on nature
                switch (eventNature)
                {
                    case EventNature.Good:
                        colorId = "green";
                        break;
                    case EventNature.Neutral:
                        colorId = "";
                        break;
                    case EventNature.Bad:
                        colorId = "red";
                        break;
                }

                return colorId;
            }


            //wraps a list of svg elements inside 1 main svg element
            //if width not set defaults to 1000px, and height to 1000px
            string WrapSvgElements(string combinedSvgString, int svgWidth = 1000, int svgTotalHeight = 1000)
            {

                //create the final svg that will be displayed
                var svgTotalWidth = svgWidth + 10; //add little for wiggle room
                var svgBody = $"<svg id=\"DasaViewHolder\" " +
                              $"style=\"" +
                              $"width:{svgTotalWidth}px;" +
                              $"height:{svgTotalHeight}px;" +
                              $"\" " +
                              $"xmlns=\"http://www.w3.org/2000/svg\">" +
                              $"{combinedSvgString}</svg>";

                return svgBody;
            }


            //generates time slices for dasa
            List<Time> GetTimeSlices()
            {
                //get time slices used to get events
                var startTime = inputPerson.BirthTime; //start time is birth time
                var endTime = startTime.AddYears(120); //end time is 120 years from birth (dasa cycle)
                var timeSlices = EventManager.GetTimeListFromRange(startTime, endTime, _timeSlicePrecision);

                return timeSlices;
            }

        }

        /// <summary>
        /// The massive method that generates every inch of the dasa svg report
        /// Note : the number of days a pixel is the zoom level
        /// </summary>
        private static async Task<string> GetDasaReportSvgFromApi2(Person inputPerson, Time startTime, Time endTime, double daysPerPixel)
        {

            //px width & height of each slice of time
            //used when generating dasa rows
            //note: changes needed only here
            int _widthPerSlice = 1;
            int _heightPerSlice = 50;
            //var lineHeight = 170;


            // One precision value for generating all dasa components,
            // because misalignment occurs if use different precision
            // note: precision = time slice count, each slice = 1 pixel (zoom level)
            // 120 years @ 14 day/px
            // 1 year @ 1 day/px 
            double eventsPrecision = Tools.DaysToHours(daysPerPixel);
            //double eventsPrecision = Tools.DaysToHours(1);


            double _timeSlicePrecision = eventsPrecision;


            //var location = inputPerson.GetBirthLocation();
            //var startTime = new Time(); //start time is birth time
            //var endTime = startTime.AddYears(120); //end time is 120 years from birth (dasa cycle)

            //use the inputed data to get events from API
            //note: below methods access the data internally
            var dasaEventList = await GetDasaEvents(eventsPrecision, startTime, endTime, inputPerson);
            var bhuktiEventList = await GetBhuktiEvents(eventsPrecision, startTime, endTime, inputPerson);
            var antaramEventList = await GetAntaramEvents(eventsPrecision, startTime, endTime, inputPerson);

            //generate rows and pump them final svg string
            var dasaSvgWidth = 0; //will be filled when calling row generator
            var compiledRow = "";

            //generate time slice only once for all rows
            var timeSlices = GetTimeSlices();

            //save a copy of the number of time slices used to calculate the svg total width later
            dasaSvgWidth = timeSlices.Count;

            //note: y axis positions are manually set
            var padding = 2;//space between rows
            int yearY = 0, yearH = 11;
            compiledRow += GenerateYearRowSvg(dasaEventList, timeSlices, eventsPrecision, yearY, 0, yearH);
            //only show month row when there is space,
            //below 1.3 days/px, anything above month names get crammed in
            int monthY = yearY, monthH = yearH;
            if (daysPerPixel <= 1.3)
            {
                monthY = yearY + yearH + padding;
                monthH = 11;
                compiledRow += GenerateMonthRowSvg(dasaEventList, timeSlices, eventsPrecision, monthY, 0, monthH);
            }
            int dasaY = monthY + monthH + padding;
            compiledRow += GenerateRowSvg(dasaEventList, timeSlices, eventsPrecision, dasaY, 0, _heightPerSlice);
            int bhuktiY = dasaY + _heightPerSlice + padding;
            compiledRow += GenerateRowSvg(bhuktiEventList, timeSlices, eventsPrecision, bhuktiY, 0, _heightPerSlice);
            int antaramY = bhuktiY + _heightPerSlice + padding;
            compiledRow += GenerateRowSvg(antaramEventList, timeSlices, eventsPrecision, antaramY, 0, _heightPerSlice);


            //the height for all lines, cursor, now & life events line
            var lineHeight = antaramY + _heightPerSlice + padding;

            //add in the cursor line
            compiledRow += $"<rect id=\"CursorLine\" width=\"2\" height=\"{lineHeight}\" style=\"fill:#000000;\" x=\"0\" y=\"0\" />";

            //get now line position
            var nowLinePosition = GetLinePosition(timeSlices, DateTimeOffset.Now);
            compiledRow += $"<rect id=\"NowVerticalLine\" width=\"2\" height=\"{lineHeight}\" style=\"fill:blue;\" x=\"0\" y=\"0\" transform=\"matrix(1, 0, 0, 1, {nowLinePosition}, 0)\" />";

            //wait!, add in life events also
            compiledRow += GetLifeEventLinesSvg(inputPerson, lineHeight);

            //compile the final svg
            var finalSvg = WrapSvgElements(compiledRow, dasaSvgWidth, (_heightPerSlice * 3) + 60); //little wiggle room

            return finalSvg;



            //█░░ █▀█ █▀▀ ▄▀█ █░░   █▀▀ █░█ █▄░█ █▀▀ ▀█▀ █ █▀█ █▄░█ █▀
            //█▄▄ █▄█ █▄▄ █▀█ █▄▄   █▀░ █▄█ █░▀█ █▄▄ ░█░ █ █▄█ █░▀█ ▄█


            //gets person's life events as lines for the dasa chart
            string GetLifeEventLinesSvg(Person person, int lineHeight)
            {
                var compiledLines = "";

                foreach (var lifeEvent in person.LifeEventList)
                {

                    //get start time of life event and find the position of it in slices (same as now line)
                    //so that this life event line can be placed exactly on the report where it happened
                    var startTime = DateTimeOffset.ParseExact(lifeEvent.StartTime, Time.GetDateTimeFormat(), null);
                    var positionX = GetLinePosition(timeSlices, startTime);

                    //note: this is the icon below the life event line to magnify it
                    var iconWidth = 12;
                    var iconX = $"-{12}"; //use negative to move center under line
                    var iconSvg = $@"
                                    <g transform=""matrix(2, 0, 0, 2, {iconX}, {lineHeight})"">
                                        <rect style=""fill: rgb(255, 242, 0); stroke: rgb(0, 0, 0); stroke-width: 0.495978px;"" x=""0"" y=""0"" width=""{iconWidth}"" height=""9.941"" rx=""2.5"" ry=""2.5""/>
                                        <path d=""M 7.823 5.279 L 6.601 5.279 C 6.377 5.279 6.194 5.456 6.194 5.671 L 6.194 6.846 C 6.194 7.062 6.377 7.238 6.601 7.238 L 7.823 7.238 C 8.046 7.238 8.229 7.062 8.229 6.846 L 8.229 5.671 C 8.229 5.456 8.046 5.279 7.823 5.279 Z M 7.823 1.364 L 7.823 1.756 L 4.566 1.756 L 4.566 1.364 C 4.566 1.148 4.383 0.973 4.158 0.973 C 3.934 0.973 3.751 1.148 3.751 1.364 L 3.751 1.756 L 3.345 1.756 C 2.892 1.756 2.534 2.108 2.534 2.539 L 2.53 8.021 C 2.53 8.454 2.895 8.804 3.345 8.804 L 9.044 8.804 C 9.492 8.804 9.857 8.452 9.857 8.021 L 9.857 2.539 C 9.857 2.108 9.492 1.756 9.044 1.756 L 8.636 1.756 L 8.636 1.364 C 8.636 1.148 8.453 0.973 8.229 0.973 C 8.006 0.973 7.823 1.148 7.823 1.364 Z M 8.636 8.021 L 3.751 8.021 C 3.528 8.021 3.345 7.845 3.345 7.629 L 3.345 3.714 L 9.044 3.714 L 9.044 7.629 C 9.044 7.845 8.86 8.021 8.636 8.021 Z"" />
                                    </g>";

                    //put together icon + line + event data
                    compiledLines += $"<g" +
                                     $" eventName=\"{lifeEvent.Name}\" " +
                                     $" age=\"{inputPerson.GetAge(startTime.Year)}\" " +
                                     $" stdTime=\"{startTime:dd/MM/yyyy}\" " + //show only date
                                     $" transform=\"matrix(1, 0, 0, 1, {positionX}, 0)\"" +
                                    $" x=\"0\" y=\"0\" >" +
                                        $"<rect" +
                                        $" width=\"2\"" +
                                        $" height=\"{lineHeight}\"" +
                                        $" style=\"fill:#fff200;\"" +
                                        $" />"
                                         + iconSvg +
                                    "</g>";

                }


                return compiledLines;
            }

            //gets line position given a date
            //finds most closest time slice, else return 0 means none found
            int GetLinePosition(List<Time> timeSliceList, DateTimeOffset inputTime)
            {
                //if nearest day is possible then end here
                var nearestDay = GetNearestDay();
                if (nearestDay != 0) { return nearestDay; }

                //else try get nearest month
                var nearestMonth = GetNearestMonth();
                if (nearestMonth != 0) { return nearestMonth; }

                //if control reaches here then now time not found in time slices
                //this is possible when viewing old charts as such set now line to 0
                return 0;

                int GetNearestMonth()
                {
                    var nowYear = inputTime.Year;
                    var nowMonth = inputTime.Month;

                    //go through the list and find where the slice is closest to now
                    var slicePosition = 0;
                    foreach (var time in timeSliceList)
                    {

                        //if same year and same month then send this slice position
                        //as the correct one
                        var sameYear = time.GetStdYear() == nowYear;
                        var sameMonth = time.GetStdMonth() == nowMonth;
                        if (sameMonth && sameYear)
                        {
                            return slicePosition;
                        }

                        //move to next slice position
                        slicePosition++;
                    }

                    return 0;
                }
                int GetNearestDay()
                {
                    var nowDay = inputTime.Day;
                    var nowYear = inputTime.Year;
                    var nowMonth = inputTime.Month;

                    //go through the list and find where the slice is closest to now
                    var slicePosition = 0;
                    foreach (var time in timeSliceList)
                    {

                        //if same year and same month then send this slice position
                        //as the correct one
                        var sameDay = time.GetStdDay() == nowDay;
                        var sameYear = time.GetStdYear() == nowYear;
                        var sameMonth = time.GetStdMonth() == nowMonth;
                        if (sameMonth && sameYear && sameDay)
                        {
                            return slicePosition;
                        }

                        //move to next slice position
                        slicePosition++;
                    }

                    return 0;
                }
            }


            string GenerateRowSvg(List<Event> eventList, List<Time> timeSlices, double precisionHours, int yAxis, int xAxis, int rowHeight)
            {
                //generate the row for each time slice
                var rowHtml = "";
                var horizontalPosition = 0; //distance from left
                var prevEventName = EventName.EmptyEvent;

                //generate 1px (rect) per time slice
                foreach (var slice in timeSlices)
                {
                    //get event that occurred at this time slice
                    //if more than 1 event raise alarm, since 1px (rect) is equal to 1 event at a time 
                    var foundEventList = eventList.FindAll(tempEvent => tempEvent.IsOccurredAtTime(slice));
                    if (foundEventList.Count > 1) throw new Exception("Only 1 event in 1 time slice!");
                    var foundEvent = foundEventList[0];

                    //if current event is different than event has changed, so draw a black line
                    var isNewEvent = prevEventName != foundEvent.Name;
                    var color = isNewEvent ? "black" : GetEventColor(foundEvent?.Nature);
                    prevEventName = foundEvent.Name;

                    //generate and add to row
                    //the hard coded attribute names used here are used in App.js
                    var rect = $"<rect " +
                               $"eventName=\"{foundEvent?.FormattedName}\" " +
                               $"age=\"{inputPerson.GetAge(slice)}\" " +
                               $"stdTime=\"{slice.GetStdDateTimeOffset():dd/MM/yyyy}\" " + //show only date
                               $"x=\"{horizontalPosition}\" " +
                               $"width=\"{_widthPerSlice}\" " +
                               $"height=\"{rowHeight}\" " +
                               $"fill=\"{color}\" />";

                    //set position for next element
                    horizontalPosition += _widthPerSlice;

                    rowHtml += rect;

                }

                //wrap all the rects inside a svg so they can me moved together
                //svg tag here acts as group, svg nesting
                rowHtml = $"<g transform=\"matrix(1, 0, 0, 1, {xAxis}, {yAxis})\">{rowHtml}</g>";


                return rowHtml;
            }

            string GenerateYearRowSvg(List<Event> eventList, List<Time> timeSlices, double precisionHours, int yAxis, int xAxis, int rowHeight)
            {

                //generate the row for each time slice
                var rowHtml = "";
                var previousYear = 0; //start 0 for first draw
                var yearBoxWidthCount = 0;
                int rectWidth = 0;
                int childAxisX = 0;
                //int rowHeight = 11;

                foreach (var slice in timeSlices)
                {

                    //only generate new year box when year changes or at
                    //end of time slices to draw the last year box
                    var lastTimeSlice = timeSlices.IndexOf(slice) == timeSlices.Count - 1;
                    var yearChanged = previousYear != slice.GetStdYear();
                    if (yearChanged || lastTimeSlice)
                    {
                        //and it is in the beginning
                        if (previousYear == 0)
                        {
                            yearBoxWidthCount = 0; //reset width
                        }
                        else
                        {
                            //generate previous year data first before resetting
                            childAxisX += rectWidth; //use previous rect width to position this
                            rectWidth = yearBoxWidthCount * _widthPerSlice; //calculate new rect width
                            var textX = rectWidth / 2; //center of box divide 2
                            var rect = $"<g transform=\"matrix(1, 0, 0, 1, {childAxisX}, 0)\">" + //y is 0 because already set in parent group
                                                $"<rect " +
                                                    $"fill=\"#0d6efd\" x=\"0\" y=\"0\" width=\"{rectWidth}\" height=\"{rowHeight}\" " + $" style=\"paint-order: stroke; stroke: rgb(255, 255, 255); stroke-opacity: 1; stroke-linejoin: round;\"/>" +
                                                    $"<text x=\"{textX}\" y=\"{9}\" width=\"{rectWidth}\" fill=\"white\"" +
                                                        $" style=\"fill: rgb(255, 255, 255);" +
                                                        $" font-size: 10px;" +
                                                        $" font-weight: 700;" +
                                                        $" text-anchor: middle;" +
                                                        $" white-space: pre;\"" +
                                                        //$" transform=\"matrix(0.966483, 0, 0, 0.879956, 2, -6.779947)\"" +
                                                        $">" +
                                                        $"{previousYear}" + //previous year generate at begin of new year
                                                    $"</text>" +
                                             $"</g>";


                            //add to final return
                            rowHtml += rect;

                            //reset width
                            yearBoxWidthCount = 0;

                        }
                    }
                    //year same as before
                    else
                    {
                        //update width only, position is same
                        //as when created the year box
                        //yearBoxWidthCount *= _widthPerSlice;

                    }

                    //update previous year for next slice
                    previousYear = slice.GetStdYear();

                    yearBoxWidthCount++;


                }

                //wrap all the rects inside a svg so they can me moved together
                //svg tag here acts as group, svg nesting
                rowHtml = $"<g transform=\"matrix(1, 0, 0, 1, {xAxis}, {yAxis})\">{rowHtml}</g>";

                return rowHtml;
            }

            string GenerateMonthRowSvg(List<Event> eventList, List<Time> timeSlices, double precisionHours, int yAxis, int xAxis, int rowHeight)
            {

                //generate the row for each time slice
                var rowHtml = "";
                var previousMonth = 0; //start 0 for first draw
                var yearBoxWidthCount = 0;
                int rectWidth = 0;
                int childAxisX = 0;
                //int rowHeight = 11;

                foreach (var slice in timeSlices)
                {

                    //only generate new year box when year changes or at
                    //end of time slices to draw the last year box
                    var lastTimeSlice = timeSlices.IndexOf(slice) == timeSlices.Count - 1;
                    var monthChanged = previousMonth != slice.GetStdMonth();
                    if (monthChanged || lastTimeSlice)
                    {
                        //and it is in the beginning
                        if (previousMonth == 0)
                        {
                            yearBoxWidthCount = 0; //reset width
                        }
                        else
                        {
                            //generate previous month data first before resetting
                            childAxisX += rectWidth; //use previous rect width to position this
                            rectWidth = yearBoxWidthCount * _widthPerSlice; //calculate new rect width
                            var textX = rectWidth / 2; //center of box divide 2
                            var rect = $"<g transform=\"matrix(1, 0, 0, 1, {childAxisX}, 0)\">" + //y is 0 because already set in parent group
                                       $"<rect " +
                                       $"fill=\"#0d6efd\" x=\"0\" y=\"0\" width=\"{rectWidth}\" height=\"{rowHeight}\" " + $" style=\"paint-order: stroke; stroke: rgb(255, 255, 255); stroke-opacity: 1; stroke-linejoin: round;\"/>" +
                                       $"<text x=\"{textX}\" y=\"{9}\" width=\"{rectWidth}\" fill=\"white\"" +
                                       $" style=\"fill: rgb(255, 255, 255);" +
                                       $" font-size: 10px;" +
                                       $" font-weight: 700;" +
                                       $" text-anchor: middle;" +
                                       $" white-space: pre;\"" +
                                       //$" transform=\"matrix(0.966483, 0, 0, 0.879956, 2, -6.779947)\"" +
                                       $">" +
                                       $"{GetMonthName(previousMonth)}" + //previous year generate at begin of new year
                                       $"</text>" +
                                       $"</g>";


                            //add to final return
                            rowHtml += rect;

                            //reset width
                            yearBoxWidthCount = 0;

                        }
                    }
                    //year same as before
                    else
                    {
                        //update width only, position is same
                        //as when created the year box
                        //yearBoxWidthCount *= _widthPerSlice;

                    }

                    //update previous month for next slice
                    previousMonth = slice.GetStdMonth();

                    yearBoxWidthCount++;


                }

                //wrap all the rects inside a svg so they can me moved together
                //svg tag here acts as group, svg nesting
                rowHtml = $"<g transform=\"matrix(1, 0, 0, 1, {xAxis}, {yAxis})\">{rowHtml}</g>";

                return rowHtml;

                string GetMonthName(int monthNum)
                {
                    switch (monthNum)
                    {
                        case 1: return "JAN";
                        case 2: return "FEB";
                        case 3: return "MAR";
                        case 4: return "APR";
                        case 5: return "MAY";
                        case 6: return "JUN";
                        case 7: return "JUL";
                        case 8: return "AUG";
                        case 9: return "SEP";
                        case 10: return "OCT";
                        case 11: return "NOV";
                        case 12: return "DEC";
                        default: throw new Exception($"Invalid Month: {monthNum}");
                    }
                }
            }


            // Get dasa color based on nature & number of events
            string GetEventColor(EventNature? eventNature)
            {
                var colorId = "gray";

                if (eventNature == null) { return colorId; }

                //set color id based on nature
                switch (eventNature)
                {
                    case EventNature.Good:
                        colorId = "green";
                        break;
                    case EventNature.Neutral:
                        colorId = "";
                        break;
                    case EventNature.Bad:
                        colorId = "red";
                        break;
                }

                return colorId;
            }


            //wraps a list of svg elements inside 1 main svg element
            //if width not set defaults to 1000px, and height to 1000px
            string WrapSvgElements(string combinedSvgString, int svgWidth = 1000, int svgTotalHeight = 1000)
            {

                //create the final svg that will be displayed
                var svgTotalWidth = svgWidth + 10; //add little for wiggle room
                var svgBody = $"<svg id=\"DasaViewHolder\" " +
                              $"style=\"" +
                              $"width:{svgTotalWidth}px;" +
                              $"height:{svgTotalHeight}px;" +
                              $"\" " +
                              $"xmlns=\"http://www.w3.org/2000/svg\">" +
                              $"{combinedSvgString}</svg>";

                return svgBody;
            }


            //generates time slices for dasa
            List<Time> GetTimeSlices() => EventManager.GetTimeListFromRange(startTime, endTime, _timeSlicePrecision);


        }

        public static async Task<List<Event>?> GetDasaEvents(double _eventsPrecision, Time startTime, Time endTime, Person person)
            => await EventsByTag(EventTag.Dasa, _eventsPrecision, startTime, endTime, person);

        public static async Task<List<Event>?> GetBhuktiEvents(double _eventsPrecision, Time startTime, Time endTime, Person person)
            => await EventsByTag(EventTag.Bhukti, _eventsPrecision, startTime, endTime, person);

        public static async Task<List<Event>?> GetAntaramEvents(double _eventsPrecision, Time startTime, Time endTime, Person person)
            => await EventsByTag(EventTag.Antaram, _eventsPrecision, startTime, endTime, person);

        /// <summary>
        /// gets events from server filtered by event tag
        /// </summary>
        public static async Task<List<Event>?> EventsByTag(EventTag tag, double precisionHours, Time startTime, Time endTime, Person person)
        {

            //get events from API server
            var dasaEventsUnsorted =
                await GetEventsFromApi(
                    startTime,
                    endTime,
                    //birth location always as current place,
                    //since place does not matter for Dasa
                    person.GetBirthLocation(),
                    person,
                    tag,
                    precisionHours);


            //sort the list by time before sending view
            var orderByAscResult = from dasaEvent in dasaEventsUnsorted
                                   orderby dasaEvent.StartTime.GetStdDateTimeOffset()
                                   select dasaEvent;


            //send sorted events to view
            return orderByAscResult.ToList();
        }

        /// <summary>
        /// Gets Muhurtha events from API
        /// </summary>
        public static async Task<List<Event>> GetEventsFromApi(Time startTime, Time endTime, GeoLocation location, Person person, EventTag tag, double precisionHours)
        {
            //prepare data to send to API
            var root = new XElement("Root");

            root.Add(
                new XElement("StartTime", startTime.ToXml()),
                new XElement("EndTime", endTime.ToXml()),
                location.ToXml(),
                person.ToXml(),
                Tools.AnyTypeToXml(tag),
                Tools.AnyTypeToXml(precisionHours));

            //get person list from storage
            var eventDataListClient = await GetFileFromContainer("EventDataList.xml", "vedastro-site-data");
            var eventDataListXml = APITools.BlobClientToXml(eventDataListClient);


            //calculate events from the data received
            var events = CalculateEvents(startTime, endTime, location, person, tag, precisionHours, eventDataListXml);

            return events;

            ////send to api and get results
            //var resultsRaw = await ServerManager.WriteToServer(ServerManager.GetEventsApi, root);


            ////parse raw results
            //List<Event> resultsParsed = Event.FromXml(resultsRaw);

            ////send to caller
            //return resultsParsed;
        }

        public static byte[] streamToByteArray(Stream input)
        {
            //reset stream position
            input.Position = 0;
            MemoryStream ms = new MemoryStream();
            input.CopyTo(ms);
            return ms.ToArray();
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }



        //▄▀█ ▀█ █░█ █▀█ █▀▀   █▀ ▀█▀ █▀█ █▀█ ▄▀█ █▀▀ █▀▀   █▀▀ █░█ █▄░█ █▀▀ ▀█▀ █ █▀█ █▄░█ █▀
        //█▀█ █▄ █▄█ █▀▄ ██▄   ▄█ ░█░ █▄█ █▀▄ █▀█ █▄█ ██▄   █▀░ █▄█ █░▀█ █▄▄ ░█░ █ █▄█ █░▀█ ▄█


        private static async Task<BlobClient> GetFileFromContainer(string fileName, string blobContainerName)
        {
            //get the connection string stored separately (for security reasons)
            var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            //get image from storage
            var blobContainerClient = new BlobContainerClient(storageConnectionString, blobContainerName);
            var fileBlobClient = blobContainerClient.GetBlobClient(fileName);

            return fileBlobClient;

            //var returnStream = new MemoryStream();
            //await fileBlobClient.DownloadToAsync(returnStream);

            //return returnStream;
        }


    }
}
