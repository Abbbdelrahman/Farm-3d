using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance {  get; private set; }

    [Header("Internal Clock")]
    [SerializeField]
    GameTimeStamp timestamp;

    public float timeScale = 1.0f;

    [Header("Day and Night cycle")]

    //the transform of the directional light (sun)
    public Transform sunTransform;

    //list of objects to inform of changes to the time
    List<ITimeTracker> listeners = new List<ITimeTracker>();

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //initialise the time stamp
        timestamp = new GameTimeStamp(0, GameTimeStamp.Season.Spring, 1, 6, 0);
        StartCoroutine(TimeUpdate());
    }

    IEnumerator TimeUpdate()
    {
        while (true)
        {
            Tick();
            yield return new WaitForSeconds(1/ timeScale);
        }
    }

    //a tick of the in-game time
    public void Tick()
    {
        timestamp.UpdateClock();

        //inform each of the listeners of the new time state
        foreach(ITimeTracker listener in listeners)
        {
            listener.ClockUpdate(timestamp);
        }
        UpdateSunMovement();
    }

    //day and night cycle
    void UpdateSunMovement()
    {
        //convert the current time to minutes
        int timeInMinutes = GameTimeStamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        //sun moves 15 degrees in an hour
        //.25 degrees in a minute 
        //at midnight (0:00), the angle of the sun should be -90
        float sunAngle = .24f * timeInMinutes - 90;

        // apply the angle to the directional light 
        sunTransform.eulerAngles = new Vector3(sunAngle, 0, 0);
    }

    //get the timestamp
    public GameTimeStamp GetGameTimestamp()
    {
        //return a cloned instance
        return new GameTimeStamp(timestamp);
    }

    //handling listeners 

    //add the object to the list of listeners
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }

    //remove teh object from the list of listeners
    public void UnregisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }
}
