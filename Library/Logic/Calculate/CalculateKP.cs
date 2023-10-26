﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SwissEphNet;
using VedAstro.Library;
using static VedAstro.Library.Calculate;

namespace VedAstro.Library
{
    public static class CalculateKP
    {

        #region WEBSITE METHODS

        /// <summary>
        /// Gets all KP (Placidus) House Cusps (start longitude) zodiac signs for all houses
        /// </summary>
        public static Dictionary<HouseName, ZodiacSign> AllHouseZodiacSignsKP(Time birthTime, int horaryNumber)
        {
            //get house start longitudes for KP system
            var allHouseCuspsRaw = HouseCuspLongitudesHorary(birthTime, horaryNumber);

            //list of house name & start longitude of house
            var returnDictionary = new Dictionary<HouseName, ZodiacSign>();

            foreach (var cuspData in allHouseCuspsRaw)
            {
                //get zodiac sign at house start longitude longitude
                var zodiacSignAtLong = ZodiacSignAtLongitude(cuspData.Value);
                var houseName = cuspData.Key;
                returnDictionary.Add(houseName, zodiacSignAtLong);

            }

            return returnDictionary;

        }

        public static Dictionary<PlanetName, PlanetName> AllPlanetLordOfConstellationKP(Time birthTime, int horNum)
        {
            // Calculate the cusps (boundaries between houses in an astrological chart) based on the birth time and horoscope number.
            var cusps = HouseCuspLongitudesHorary(birthTime, horNum);

            // For each planet, calculate its Nirayana longitude (sidereal longitude in Vedic astrology) at the birth time.
            // Filter out the planets that are not in any of the houses.
            // Then, create a dictionary mapping each planet to its constellation lord at that longitude.
            var allPlanetZodiacSignKp = PlanetName.All9Planets
                .Select(planet => new
                {
                    Planet = planet,
                    NirayanaDegrees = PlanetNirayanaLongitude(birthTime, planet)
                })
                .Where(planetData => Enumerable.Range(1, cusps.Count)
                    .Any(houseIndex => IsPlanetInHouseKP(cusps, planetData.NirayanaDegrees, houseIndex)))
                .ToDictionary(
                    //planet name set as KEY
                    planetData => planetData.Planet,
                    //constellation lord set as VALUE
                    planetData => LordOfConstellation(ConstellationAtLongitude(planetData.NirayanaDegrees).GetConstellationName()));

            return allPlanetZodiacSignKp;
        }

        public static Dictionary<PlanetName, PlanetConstellation> AllPlanetConstellationKP(Time birthTime, int horNum)
        {
            // Calculate the cusps (boundaries between houses in an astrological chart) based on the birth time and horoscope number.
            var cusps = HouseCuspLongitudesHorary(birthTime, horNum);

            // For each planet, calculate its Nirayana longitude (sidereal longitude in Vedic astrology) at the birth time.
            // Filter out the planets that are not in any of the houses.
            // Then, create a dictionary mapping each planet to its constellation at that longitude.
            var allPlanetZodiacSignKp = PlanetName.All9Planets
                .Select(planet => new
                {
                    Planet = planet,
                    NirayanaDegrees = PlanetNirayanaLongitude(birthTime, planet)
                })
                .Where(planetData => Enumerable.Range(1, cusps.Count)
                    .Any(houseIndex => IsPlanetInHouseKP(cusps, planetData.NirayanaDegrees, houseIndex)))
                .ToDictionary(
                    //planet name set as KEY
                    planetData => planetData.Planet,
                    //constellation set as VALUE
                    planetData => ConstellationAtLongitude(planetData.NirayanaDegrees));

            return allPlanetZodiacSignKp;
        }

        public static Dictionary<PlanetName, PlanetName> AllPlanetLordOfZodiacSignKP(Time birthTime, int horNum)
        {
            // Calculate the cusps (boundaries between houses in an astrological chart) based on the birth time and horoscope number.
            var cusps = HouseCuspLongitudesHorary(birthTime, horNum);

            // For each planet, calculate its Nirayana longitude (sidereal longitude in Vedic astrology) at the birth time.
            // Filter out the planets that are not in any of the houses.
            // Then, create a dictionary mapping each planet to its Lord of Zodiac Sign at that longitude.
            var allPlanetZodiacSignKp = PlanetName.All9Planets
                .Select(planet => new
                {
                    Planet = planet,
                    NirayanaDegrees = PlanetNirayanaLongitude(birthTime, planet)
                })
                .Where(planetData => Enumerable.Range(1, cusps.Count)
                    .Any(houseIndex => IsPlanetInHouseKP(cusps, planetData.NirayanaDegrees, houseIndex)))
                .ToDictionary(
                    //planet name set as KEY
                    planetData => planetData.Planet,
                    //Lord of zodiac sign set as VALUE
                    planetData =>
                    {
                        var zodiacSign = ZodiacSignAtLongitude(planetData.NirayanaDegrees);
                        return LordOfZodiacSign(zodiacSign.GetSignName());
                    });

            return allPlanetZodiacSignKp;
        }

        public static Dictionary<PlanetName, ZodiacSign> AllPlanetZodiacSignKP(Time birthTime, int horNum)
        {
            // Calculate the cusps (boundaries between houses in an astrological chart) based on the birth time and horoscope number.
            var cusps = HouseCuspLongitudesHorary(birthTime, horNum);

            // For each planet, calculate its Nirayana longitude (sidereal longitude in Vedic astrology) at the birth time.
            // Filter out the planets that are not in any of the houses.
            // Then, create a dictionary mapping each planet to its Zodiac Sign at that longitude.
            var allPlanetZodiacSignKp = PlanetName.All9Planets
                .Select(planet => new
                {
                    Planet = planet,
                    NirayanaDegrees = PlanetNirayanaLongitude(birthTime, planet)
                })
                .Where(planetData => Enumerable.Range(1, cusps.Count)
                    .Any(houseIndex => IsPlanetInHouseKP(cusps, planetData.NirayanaDegrees, houseIndex)))
                .ToDictionary(
                    //planet name set as KEY
                    planetData => planetData.Planet,
                    //zodiac sign set as VALUE
                    planetData => ZodiacSignAtLongitude(planetData.NirayanaDegrees)
                );

            return allPlanetZodiacSignKp;
        }

        public static Dictionary<PlanetName, HouseName> AllPlanetHouseKP(Time birthTime, int horNum)
        {
            //get house start longitudes
            var cusps = HouseCuspLongitudesHorary(birthTime, horNum);

            var allPlanetHouseKp = PlanetName.All9Planets.ToDictionary(
                // The first function takes a 'planet' as a parameter and simply returns it. This is used to generate the keys of the dictionary.
                planet => planet,

                // The second function also takes a 'planet' as a parameter. It returns the first 'houseIndex' for which 'IsPlanetInHouseKP' returns true.
                // 'IsPlanetInHouseKP' is called with 'cusps', the result of 'Calculate.PlanetNirayanaLongitude(birthtime, planet)', and 'houseIndex' as parameters.
                planet => (HouseName)Enumerable.Range(1, cusps.Count)
                    .First(houseIndex => IsPlanetInHouseKP(cusps, PlanetNirayanaLongitude(birthTime, planet), houseIndex)));

            return allPlanetHouseKp;
        }

        #endregion


        #region KP SPECIFIC CALCS

        public static bool IsPlanetInHouseKP(Dictionary<HouseName, Angle> cusps, Angle planetNirayanaDegrees, int houseIndex)
        {
            if (houseIndex + 1 < cusps.Count) //Do not exceed the bounds of the array
            {
                //check if cusp longitude is smaller than next cusp longitude
                if (cusps[(HouseName)houseIndex + 1] > cusps[(HouseName)houseIndex])
                {
                    return (planetNirayanaDegrees.TotalDegrees >= cusps[(HouseName)houseIndex].TotalDegrees) &&
                           //this means that the planet falls in between these house cusps
                           (planetNirayanaDegrees.TotalDegrees <= cusps[(HouseName)houseIndex + 1].TotalDegrees); 
                }

                //if next cusp start long is smaller than current cusp we are rotating through 360 deg
                else
                {
                    return (planetNirayanaDegrees.TotalDegrees >= cusps[(HouseName)houseIndex].TotalDegrees) ||
                           (planetNirayanaDegrees.TotalDegrees <= cusps[(HouseName)houseIndex + 1].TotalDegrees);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all KP (Placidus) House Cusps 
        /// using Swiss Epehemris swe_houses_ex
        /// </summary>
        public static Dictionary<HouseName, Angle> HouseCuspLongitudesHorary(Time time, int horaryNumber)
        {
            //get location at place of time
            var location = time.GetGeoLocation();

            SwissEph swissEph = new SwissEph();

            double[] cusps = new double[13];

            //we have to supply ascmc to make the function run
            double[] ascmc = new double[10];

            //set ayanamsa
            swissEph.swe_set_sid_mode(Calculate.Ayanamsa, 0, 0);

            var eps = EclipticObliquity(time);

            //convert horary number inputed by user to Tropical Ascendant degrees
            var tropAsc = HoraryNumberTropicalAsc(horaryNumber);
            var armc = ConvertTropicalAscToARMC(tropAsc, eps, location.Latitude(), time);

            var lat = location.Latitude();
            swissEph.swe_houses_armc(armc, lat, eps, 'P', cusps, ascmc);


            //package data before sending
            var housesDictionary = new Dictionary<HouseName, Angle>();
            foreach (var house in House.AllHouses)
            {
                //start of house longitude of 0-360
                var hseLong = cusps[(int)house];
                housesDictionary.Add(house, Angle.FromDegrees(hseLong));
            }

            //return cusps;
            return housesDictionary;

        }

        /// <summary>
        /// This method is used to convert the tropical ascendant to the ARMC (Ascendant Right Meridian Circle).
        /// It first calculates the right ascension and declination using the provided tropical ascendant and
        /// obliquity of the ecliptic. Then, it calculates the oblique ascension by subtracting a value derived
        /// from the declination and geographic latitude from the right ascension. Finally, it calculates the ARMC
        /// based on the value of the tropical ascendant and the oblique ascension.
        /// </summary>
        public static double ConvertTropicalAscToARMC(double tropicalAscendant, double obliquityOfEcliptic, double geographicLatitude, Time time)
        {
            // The main method is taken from a post by K S Upendra on Group.IO in 2019
            // Calculate the right ascension using the formula:
            // atan(cos(obliquityOfEcliptic) * tan(tropicalAscendant))
            double rightAscension =
                Math.Atan(Math.Cos(obliquityOfEcliptic * Math.PI / 180) * Math.Tan(tropicalAscendant * Math.PI / 180)) *
                180 / Math.PI;
            // Calculate the declination using the formula:
            // asin(sin(obliquityOfEcliptic) * sin(tropicalAscendant))
            double declination =
                Math.Asin(Math.Sin(obliquityOfEcliptic * Math.PI / 180) * Math.Sin(tropicalAscendant * Math.PI / 180)) *
                180 / Math.PI;
            // Calculate the oblique ascension by subtracting the result of the following formula from the right ascension:
            // asin(tan(declination) * tan(geographicLatitude))
            double obliqueAscension = rightAscension -
                                      (Math.Asin(Math.Tan(declination * Math.PI / 180) *
                                                 Math.Tan(geographicLatitude * Math.PI / 180)) * 180 / Math.PI);
            // Initialize the armc variable
            double armc = 0;
            // Depending on the value of the tropical ascendant, calculate the armc using the formula:
            // armc = 270 + obliqueAscension or armc = 90 + obliqueAscension
            if (tropicalAscendant > 0 && tropicalAscendant < 90)
            {
                armc = 270 + obliqueAscension;
            }
            else if (tropicalAscendant > 90 && tropicalAscendant < 180)
            {
                armc = 90 + obliqueAscension;
            }
            else if (tropicalAscendant > 180 && tropicalAscendant < 270)
            {
                armc = 90 + obliqueAscension;
            }
            else if (tropicalAscendant > 270 && tropicalAscendant < 360)
            {
                armc = 270 + obliqueAscension;
            }
            // Return the calculated armc value
            return armc;
        }

        /// <summary>
        /// This method calculates the tropical ascendant for a given horary number.
        /// It does this by iterating over all constellations and planets, calculating
        /// various parameters before and after adding a certain degree to the tropical
        /// ascendant (tropAsc), and handling special cases such as when tropAsc is 0 or
        /// when there are overlapping signs. The tropical ascendant corresponding to the
        /// given horary number is then returned.
        /// </summary>
        public static double HoraryNumberTropicalAsc(int horaryNumber)
        {
            // Initialize variables
            var tropAscDeg = 0.00;
            var all9Planets = PlanetName.All9Planets;
            var allConstellations = PlanetConstellation.AllConstellation;
            var constellationList = new List<Tuple<int, ZodiacName, PlanetName, ConstellationName, PlanetName, PlanetName, double>>();
            var cntA = 0;
            var tropAsc = 0.000;

            // Iterate over all constellations
            foreach (var constellation in allConstellations)
            {
                var tempConstel = constellation;
                var lordofConstel = LordOfConstellation(tempConstel);
                var planetnameArray = all9Planets.ToArray();
                int cntB = 0;
                int index = 0;

                // Find the index of the lord of the constellation in the planet array
                while (cntB <= 8)
                {
                    if (planetnameArray[cntB] == lordofConstel)
                    {
                        index = cntB;
                        break;
                    }
                    cntB++;
                }
                var planetnameArrayB = new PlanetName[9];
                int cntC = 0;

                // Reorder the planet array based on the index of the lord of the constellation
                while (cntC <= 8)
                {
                    planetnameArrayB[cntC] = planetnameArray[index];
                    cntC++;
                    index++;
                    if (index == planetnameArray.Length)
                    {
                        index = 0;
                    }
                }
                // Iterate over the reordered planet array
                foreach (var planetName in planetnameArrayB)
                {
                    var constellationA = tempConstel;
                    lordofConstel = LordOfConstellation(constellationA);
                    // Assign tropAscDeg based on the planet name
                    switch (planetName.Name)
                    {
                        case PlanetName.PlanetNameEnum.Ketu:
                            tropAscDeg = new Angle(0, 46, 40).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Venus:
                            tropAscDeg = new Angle(2, 13, 20).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Sun:
                            tropAscDeg = new Angle(0, 40, 0).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Moon:
                            tropAscDeg = new Angle(1, 06, 40).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Mars:
                            tropAscDeg = new Angle(0, 46, 40).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Rahu:
                            tropAscDeg = new Angle(2, 00, 00).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Jupiter:
                            tropAscDeg = new Angle(1, 46, 40).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Saturn:
                            tropAscDeg = new Angle(2, 06, 40).TotalDegrees;
                            break;
                        case PlanetName.PlanetNameEnum.Mercury:
                            tropAscDeg = new Angle(1, 53, 20).TotalDegrees;
                            break;
                    }
                    // Calculate various parameters before and after adding tropAscDeg to tropAsc
                    var zSignAtLongBefore = ZodiacSignAtLongitude(Angle.FromDegrees(tropAsc));
                    var constellationBefore = ConstellationAtLongitude(Angle.FromDegrees(tropAsc));
                    var constellationLordBefore = LordOfConstellation(constellationBefore.GetConstellationName());
                    // Special handling for tropAsc == 0.00
                    if (tropAsc == 0.00)
                    {
                        var longBefore = tropAsc - 0.00001 + 360;
                        zSignAtLongBefore = ZodiacSignAtLongitude(Angle.FromDegrees(longBefore));
                        constellationBefore = ConstellationAtLongitude(Angle.FromDegrees(longBefore));
                        constellationLordBefore = LordOfConstellation(constellationBefore.GetConstellationName());
                    }
                    else
                    {
                        zSignAtLongBefore = ZodiacSignAtLongitude(Angle.FromDegrees(tropAsc));
                        constellationBefore = ConstellationAtLongitude(Angle.FromDegrees(tropAsc));
                        constellationLordBefore = LordOfConstellation(constellationBefore.GetConstellationName());
                    }
                    tropAsc = tropAsc + tropAscDeg;
                    var zSignAtLongAfter = ZodiacSignAtLongitude(Angle.FromDegrees(tropAsc));
                    var zSignAfterLord = LordOfZodiacSign(zSignAtLongAfter.GetSignName());
                    var constellationAfter = ConstellationAtLongitude(Angle.FromDegrees(tropAsc));
                    var constellationLordAfter = LordOfConstellation(constellationAfter.GetConstellationName());
                    // Check if tropAsc ends on 30, 60, 90, 120....
                    var tropAscSpansSigns = Math.Round(tropAsc % 30, 6);
                    // Handle overlapping signs issue
                    if ((zSignAtLongAfter.GetSignName() != zSignAtLongBefore.GetSignName()) &&
                        (constellationLordAfter == constellationLordBefore)
                        && ((zSignAtLongBefore.GetSignName() == ZodiacName.Aries) ||
                            (zSignAtLongBefore.GetSignName() == ZodiacName.Leo) ||
                            (zSignAtLongBefore.GetSignName() == ZodiacName.Sagittarius) ||
                            (zSignAtLongBefore.GetSignName() == ZodiacName.Gemini) ||
                            (zSignAtLongBefore.GetSignName() == ZodiacName.Libra) ||
                            (zSignAtLongBefore.GetSignName() == ZodiacName.Aquarius)))
                    {
                        var remainderFromDivBy30 = (tropAsc % 30.00); //past signchange degree amount
                        var preSignChangeDegrees = tropAscDeg - remainderFromDivBy30;
                        tropAsc = tropAsc - tropAscDeg + preSignChangeDegrees; //this is one Entry into the List. this get us to Sign engpoint
                        //log entry into List
                        constellationList.Add(new Tuple<int, ZodiacName, PlanetName, ConstellationName, PlanetName, PlanetName, double>(cntA + 1, zSignAtLongAfter.GetSignName(), zSignAfterLord, constellationA, constellationLordAfter, planetName, tropAsc));
                        cntA++;
                        //next process the balance into the nextSign
                        var tropAscB = tropAsc + remainderFromDivBy30;
                        //log entry into List
                        constellationList.Add(new Tuple<int, ZodiacName, PlanetName, ConstellationName, PlanetName, PlanetName, double>(cntA + 1, zSignAtLongAfter.GetSignName(), zSignAfterLord, constellationA, constellationLordAfter, planetName, tropAscB));
                        cntA++;
                        tropAsc = tropAscB;
                    }
                    else
                    {
                        constellationList.Add(new Tuple<int, ZodiacName, PlanetName, ConstellationName, PlanetName, PlanetName, double>(cntA + 1, zSignAtLongAfter.GetSignName(), zSignAfterLord, constellationA, constellationLordAfter, planetName, tropAsc));
                        cntA++;
                    }
                }
            }

            // Find the horary number in the constellation list and return the corresponding tropAsc
            var countX = 0;
            while (countX <= 248)
            {
                if (constellationList[countX].Item1 == horaryNumber)
                {
                    //NOTE:
                    //the -1 because in the list we record end longitudes.
                    //We have to return start longitudes.
                    //the end longitude of the previous one is the start of the current one. 
                    tropAsc = constellationList[countX - 1].Item7;
                }
                countX++;
            }


            return tropAsc;
        }

        #endregion

    }
}