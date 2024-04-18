using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameTimeStamp
{
    public int year;
    public enum Season
    {
        Spring, 
        Summer,
        Fall,
        Winter
    }
    public Season season;

    public enum DayOfTheWeek
    {
        Saturday,
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }

    public int day;
    public int hour;
    public int minute;

    //constructor to set up the class
    public GameTimeStamp(int year, Season season, int day, int hour, int minute)
    {
        this.year = year;
        this.season = season;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }


    //creating a new instance of a gametimestamp from another pre-existing one
    public GameTimeStamp(GameTimeStamp timeStamp)
    {
        this.year = timeStamp.year;
        this.season = timeStamp.season;
        this.day = timeStamp.day;
        this.hour = timeStamp.hour;
        this.minute = timeStamp.minute;
    }

    //increamint the time by 1 minute 
    public void UpdateClock()
    {
        minute++;
        
        //60 minutes in 1 hour
        if(minute >= 60)
        {
            //reset minutes
            minute = 0;
            hour++;
        }

        //24 hours in 1 day
        if (hour >= 24)
        {
            //reset hours
            hour = 0;
            day++;
        }

        //30 days in a season
        if(day >= 30)
        {
            //reset days
            day = 1;

            //if at the final season, reset and change to spring
            if(season == Season.Winter)
            {
                season = Season.Spring;
                //start of a new year
                year++;
            }
            else
            {
                season++;
            }
        }
    }

    public DayOfTheWeek GetDayOfTheWeek()
    {
        //convert the total time passed into days
        int daysPassed = YearsToDays(year) + SeasonsToDays(season) + day;

        //remainder after dividing dayspassed by 7
        int dayIndex = daysPassed % 7;

        //cast into day of the week
        return (DayOfTheWeek)dayIndex;
    }

    //convert hours to minutes
    public static int HoursToMinutes(int hours)
    {
        //60 minutes = 1 hour
        return hours * 60;
    }

    //convert days to hours
    public static int DaysToHours(int days)
    {
        //24 hours in a day
        return days * 24;
    }

    //convert seasons to days
    public static int SeasonsToDays(Season season)
    {
        int seasonIndex = (int)season;
        return seasonIndex * 30;
    }

    //years to days
    public static int YearsToDays(int years)
    {
        return years * 4 * 30;
    }

    //calculate the difference between 2 timestamps in hours
    public static int CompareTimestamps(GameTimeStamp timestamp1, GameTimeStamp timestamp2)
    {
        //convert timestamp to hours
        int timestamp1Hours = DaysToHours(YearsToDays(timestamp1.year)) + DaysToHours(SeasonsToDays(timestamp1.season)) + DaysToHours(timestamp1.day) + timestamp1.hour;
        int timestamp2Hours = DaysToHours(YearsToDays(timestamp2.year)) + DaysToHours(SeasonsToDays(timestamp2.season)) + DaysToHours(timestamp2.day) + timestamp2.hour;
        int difference = timestamp2Hours - timestamp1Hours;
        return Mathf.Abs(difference);
    }
}
