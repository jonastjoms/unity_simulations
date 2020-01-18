using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
class livingRoom : MonoBehaviour
{
    public int count;
    // Initialize all game objects:
    public GameObject[] humans;
    public GameObject pepper;
    public GameObject human1;
    public GameObject human2;
    public GameObject human3;
    public GameObject human4;
    public GameObject human5;
    public GameObject human6;
    public GameObject layingHuman;
    public GameObject sittingHuman;
    public GameObject child1;
    public GameObject child2;
    public GameObject dog;

    // Temporary:
    GameObject tempPepper;
    GameObject tempHuman1;
    GameObject tempHuman2;
    GameObject tempHuman3;
    GameObject tempHuman4;
    GameObject tempHuman5;
    GameObject tempHuman6;
    GameObject tempLayingHuman;
    GameObject tempSittingHuman;
    GameObject tempChild1;
    GameObject tempChild2;
    GameObject tempDog;

    // Position variables:
    // Pepper
    public float pepperXPos;
    public float pepperYPos;
    public float pepperZPos;
    public Quaternion pepperRot;
    public int pepperArea;
    // 3 humans
    private float humanXPos;
    private float humanYPos;
    private float humanZPos;
    private float humanYRot;
    private int closestHuman;
    private int closest2Human;
    private int closest3Human;
    public int humanArea;
    // Animal
    private float animalXPos;
    private float animalYPos;
    private float animalZPos;
    private float animalYRot;
    private int animalArea;
    // Children
    private float childXPos;
    private float childYPos;
    private float childZPos;
    private float childYRot;
    private int childArea;
    // Group variables
    private float groupXPos;
    private float groupYPos;
    private float groupZPos;
    public int groupArea;

    // Variables for objects instantiated:
    public int instantatedStandingHumans;
    private int standingHumanCount;

    // Variables for circle around Pepper
    [Range(0, 50)]
    int segments = 50;
    [Range(0, 1)]
    public float xradius;
    [Range(0, 1)]
    public float yradius;
    LineRenderer line;

    // Variables for arrow
    public GameObject arrow;
    GameObject tempArrow;

    // Arrow or circle
    public int usingCircle;
    public int usingAarrow;

    // Text and music variables
    public Text myText;
    public GameObject sound;
    GameObject tempSound;

    // CSV file for storing of features and screenshots
    private static string reportFileName = "features.csv";
    private static string reportSeparator = ",";
    private static string[] reportHeaders = new string[27] {
        "Stamp",
        "File Path",
        "Using circle",
        "Using arrow",
        "Number of people",
        "Number of people in group",
        "Group radius",
        "Distance to group",
        "Robot within group?",
        "Robot facing group?",
        "Robot work radius",
        "Distance to closest human",
        "Distance to 2nd closest human",
        "Distance to 3rd closest human",
        "Direction to closest human",
        "Direction to 2nd closest human",
        "Direction to 3rd closest human",
        "Direction from closest human to robot",
        "Robot facing closest human?",
        "Closest human facing robot?",
        "Number of children",
        "Distance to closest child",
        "Number of animals",
        "Distance to closest animal",
        "Number of people sitting/laying in sofa?",
        " Music playing?",
        "Number of agents in scene"
    };

    // Feauture Variables
    // For binary values 0 = False, 1 = True
    public int nPeople;
    public int nPeopleGroup; // Number of people in group
    public float groupRadius = 50;
    public float distGroup = 50; // Euclidean distance to group, if no group -> 50
    public int robotWithinGroup;  // Binary
    public float robotRadius;
    public int facingGroup;  // Binary, if no group -> 0
    public float distHuman1 = 50; // Euclidean distance to closest human, if no people -> 50
    public float distHuman2 = 50; // Euclidean distance to 2nd closest human, if no people -> 50
    public float distHuman3 = 50; // Euclidean distance to 3d closest human, if no people -> 50
    public float directionHuman1 = 1000; // Vector direction to closest human, if no people -> 1000
    public float directionHuman2 = 1000; // Vector direction to 2nd closest human, if no people -> 1000
    public float directionHuman3 = 1000; // Vector direction to 3d closest human, if no people -> 1000
    public float directionRobotHuman1 = 1000; // Vector direction to robot from closest human, if no people -> 1000
    public int robotFacingHuman1;  // Binary
    public int human1FacingRobot;  // Binary
    public int nChildren;
    public float distChildren = 50; // Euclidean distance to closest child, if no child -> 50
    public int nAnimal;
    public float distAnimal = 50; // Euclidean distance to closest animal, if no animal -> 50
    public int nPeopleSofa;
    public int musicPlaying;  // Binary
    public int agentsInScene;


    private void Start()
    {
        // Initialize Line parameters
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = (segments + 1);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.startColor = Color.white;
        line.endColor = Color.white;
        line.useWorldSpace = true;

        // Store human game objects
        List<GameObject> objects = new List<GameObject>() {human1, human2, human3, human4, human5, human6, layingHuman, sittingHuman, child1, child2, dog};
        List<GameObject> temps = new List<GameObject>() {tempHuman1, tempHuman2, tempHuman3, tempHuman4, tempHuman5, tempHuman6, tempLayingHuman, tempSittingHuman, tempChild1, tempChild2, tempDog};
        // Text
        myText = myText.GetComponent<Text>();
        // Create csv file and initiate headers:
        CreateReport();
        // Start program
        StartCoroutine(SpawnRandom(objects, temps));
    }

    private (float, float) SpawnPosition(int room_area)
    {
        float xPos;
        float zPos;
        if (room_area == 0)
        {
            xPos = Random.Range(-2.62f, -3.26f);
            zPos = Random.Range(-4.21f, -1.96f);
        }
        else if (room_area == 1)
        {
            xPos = Random.Range(-3.75f, -4.45f);
            zPos = Random.Range(-5.09f, 0.02f);
        }
        else if (room_area == 2)
        {
            xPos = Random.Range(-4.8f, -6.64f);
            zPos = Random.Range(-5.09f, -2.96f);
        }
        else
        {
            xPos = Random.Range(-6.95f, -8.65f);
            zPos = Random.Range(-2.59f, 0.02f);
        }
        return (xPos, zPos);
    }

    private (float, float) GroupPosition(int room_area)
    {
        float xPos;
        float zPos;
        if (room_area == 0)
        {
            xPos = -3f;
            zPos = -3f;
        }
        else if (room_area == 1)
        {
            xPos = -4f;
            zPos = -2.5f;
        }
        else if (room_area == 2)
        {
            xPos = -5.5f;
            zPos = -4f;
        }
        else
        {
            xPos = -7.5f;
            zPos = -1.5f;
        }
        return (xPos, zPos);
    }

    private static void AppendToReport(string[] strings)
    {
        using (StreamWriter sw = File.AppendText("data/features.csv"))
        {
            string finalString = "";
            for (int i = 0; i < strings.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += strings[i];
            }
            finalString += reportSeparator;
            sw.WriteLine(finalString);
        }
    }

    private static void CreateReport()
    {
        using (StreamWriter sw = File.CreateText("data/features.csv"))
        {
            string finalString = "";
            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (finalString != "")
                {
                    finalString += reportSeparator;
                }
                finalString += reportHeaders[i];
            }
            finalString += reportSeparator;
            sw.WriteLine(finalString);
        }
    }

    private Vector3 GroupCircle(Vector3 center, float radius, float ang)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    private bool InvalidPosition(Vector3 pos, List<GameObject> temps, Vector3 pepperPos)
    {
        // Check if colliding with Pepper
        if (Vector3.Distance(pos, pepperPos) < 0.4)
        {
            return true;
        }
         // Check if colliding with standing humans
        for (int i = 0; i < instantatedStandingHumans; i++)
        {
            if (Vector3.Distance(pos, temps[i].gameObject.transform.position) < 0.4)
                {
                    return true;
                }
        }

        // Check if colliding with children
        for (int i = 0; i < nChildren; i++)
        {
            if (Vector3.Distance(pos, temps[i+8].gameObject.transform.position) < 0.4)
            {
                return true;
            }
        }

        // Check if colliding with animal
        for (int i = 0; i < nAnimal; i++)
        {
            if (Vector3.Distance(pos, temps[i + 10].gameObject.transform.position) < 0.4)
            {
                return true;
            }
        }
        return false;


    }

    IEnumerator SpawnRandom(List<GameObject> objects, List<GameObject> temps)
    {
        while (count < 10)
        {
            instantatedStandingHumans = 0;
            agentsInScene = 0;
            nChildren = 0;
            nAnimal = 0;
            distHuman1 = 50;
            distHuman2 = 50;
            distHuman3 = 50;
            directionHuman1 = 1000;
            directionHuman2 = 1000;
            directionHuman3 = 1000;
            directionRobotHuman1 = 1000;

            if (Random.Range(0,2) == 1)
            {
                usingCircle = 1;
                usingAarrow = 0;
            }
            else
            {
                usingCircle = 0;
                usingAarrow = 1;
                robotRadius = 50;
            }

            // Determine number of standing people:
            standingHumanCount = Random.Range(0, 7);
            // Determine if group or not, only if 3 or more people
            if (standingHumanCount > 2)
            {
                nPeopleGroup = Random.Range(0, standingHumanCount);
            }

            // If group, choose area and place out people
            if (nPeopleGroup > 2) // We have a group
            {
                // Determine group area
                groupArea = Random.Range(0, 4);
                // Determmine center position
                (groupXPos, groupZPos) = GroupPosition(groupArea);
                groupYPos = -0.921f;
                Vector3 groupCenter = new Vector3(groupXPos, groupYPos, groupZPos);
                // Sample group radius
                if (groupArea == 1)
                {
                    groupRadius = Random.Range(0.5f, 0.8f);
                }
                else
                {
                    groupRadius = Random.Range(0.5f, 1f);
                }
                // Spawn people in group
                for (int i = 0; i < nPeopleGroup; i++)
                {
                    float angle = i * (360f / nPeopleGroup);
                    Vector3 pos = GroupCircle(groupCenter, groupRadius, angle);
                    Vector3 relativePos = groupCenter - pos;
                    Quaternion rot = Quaternion.LookRotation(relativePos, Vector3.up);
                    temps[i] = Instantiate(objects[i], pos, rot);
                    instantatedStandingHumans += 1;
                    agentsInScene += 1;
                }

                // Determine if Pepper is in group
                robotWithinGroup = Random.Range(0, 2);
                // Spawn Pepper
                if (robotWithinGroup == 1)
                {
                    distGroup = 0f;
                    facingGroup = 1;
                    Vector3 pepperPos = groupCenter; // Group center
                    pepperRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    pepper.gameObject.transform.localScale = new Vector3(15, 15, 15);
                    tempPepper = Instantiate(pepper, pepperPos, pepperRot);
                    agentsInScene += 1;

                    // Draw line  or arrow around Pepper
                    if (usingCircle == 1)
                    {
                        float x;
                        float y = -0.8f;
                        float z;
                        float angle = 20f;
                        xradius = Random.Range(0.5f, 3f);
                        yradius = xradius;
                        robotRadius = xradius;
                        for (int i = 0; i < (segments + 1); i++)
                        {
                            x = pepperPos[0] + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                            z = pepperPos[2] + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
                            line.SetPosition(i, new Vector3(x, y, z));
                            angle += (360f / segments);
                        }
                    }
                    else
                    {
                        // Find vector going out from pepper:
                        Vector3 tempArrowRot = tempPepper.gameObject.transform.forward;
                        Vector3 pos = pepperPos + (0.5f * tempArrowRot);
                        Quaternion arrowRot = tempPepper.gameObject.transform.rotation;
                        Vector3 temp_rot = pepperRot.eulerAngles;
                        temp_rot[0] -= 90;
                        temp_rot[1] += 90;
                        pos[1] = -0.8f;
                        arrowRot = Quaternion.Euler(temp_rot);
                        arrow.gameObject.transform.localScale = new Vector3(0.8f, 0.2f, 0.5f);
                        tempArrow = Instantiate(arrow, pos, arrowRot);

                        // Move circle out oof view
                        float x;
                        float y = -3f;
                        float z;
                        float angle = 20f;
                        xradius = Random.Range(0.5f, 3f);
                        yradius = xradius;
                        robotRadius = 0;
                        for (int i = 0; i < (segments + 1); i++)
                        {
                            x = pepperPos[0] + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                            z = pepperPos[2] + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
                            line.SetPosition(i, new Vector3(x, y, z));
                            angle += (360f / segments);
                        }
                    }

                }
                else
                {
                    // Instantiate Pepper
                    // Define spawn position
                    pepperArea = Random.Range(0, 4);
                    // Make sure not same area as group
                    while (pepperArea == groupArea)
                    {
                        pepperArea = Random.Range(0, 4);
                    }
                    (pepperXPos, pepperZPos) = SpawnPosition(pepperArea);
                    pepperYPos = -0.921f;
                    Vector3 pepperPos = new Vector3(pepperXPos, pepperYPos, pepperZPos);
                    // Should Pepper face group or not
                    facingGroup = Random.Range(0, 2);
                    if (facingGroup == 1)
                    {
                        Vector3 relativePos = groupCenter - pepperPos;
                        pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                    }
                    else
                    {
                        Vector3 relativePos = groupCenter - pepperPos;
                        pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                        Vector3 temp_rot = pepperRot.eulerAngles;
                        temp_rot[1] = temp_rot[1] + 180;
                        pepperRot = Quaternion.Euler(temp_rot);

                    }

                    pepper.gameObject.transform.localScale = new Vector3(15, 15, 15);
                    tempPepper = Instantiate(pepper, pepperPos, pepperRot);
                    agentsInScene += 1;
                    // Determine distance to group
                    distGroup = Vector3.Distance(groupCenter, pepperPos);

                    // Draw line  or arrow around Pepper
                    if (usingCircle == 1)
                    {
                        float x;
                        float y = -0.8f;
                        float z;
                        float angle = 20f;
                        xradius = Random.Range(0.5f, 3f);
                        yradius = xradius;
                        robotRadius = xradius;
                        for (int i = 0; i < (segments + 1); i++)
                        {
                            x = pepperPos[0] + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                            z = pepperPos[2] + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
                            line.SetPosition(i, new Vector3(x, y, z));
                            angle += (360f / segments);
                        }
                    }
                    else
                    {
                        // Find vector going out from pepper:
                        Vector3 tempArrowRot = tempPepper.gameObject.transform.forward;
                        Vector3 pos = pepperPos + (0.5f * tempArrowRot);
                        Quaternion arrowRot = tempPepper.gameObject.transform.rotation;
                        Vector3 temp_rot = pepperRot.eulerAngles;
                        temp_rot[0] -= 90;
                        temp_rot[1] += 90;
                        pos[1] = -0.8f;
                        arrowRot = Quaternion.Euler(temp_rot);
                        arrow.gameObject.transform.localScale = new Vector3(0.8f, 0.2f, 0.5f);
                        tempArrow = Instantiate(arrow, pos, arrowRot);

                        // Move circle out of view
                        float x;
                        float y = -3f;
                        float z;
                        float angle = 20f;
                        xradius = Random.Range(0.5f, 3f);
                        yradius = xradius;
                        robotRadius = 0;
                        for (int i = 0; i < (segments + 1); i++)
                        {
                            x = pepperPos[0] + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                            z = pepperPos[2] + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
                            line.SetPosition(i, new Vector3(x, y, z));
                            angle += (360f / segments);
                        }
                    }
                }
            }
            else
            {
                nPeopleGroup = 0;
                groupRadius = 50;
                // Instantiate Pepper
                // Define spawn position
                pepperArea = Random.Range(0, 4);
                (pepperXPos, pepperZPos) = SpawnPosition(pepperArea);
                pepperYPos = -0.921f;
                Vector3 pepperPos = new Vector3(pepperXPos, pepperYPos, pepperZPos);
                // Should Pepper face group or not
                facingGroup = 0;
                pepper.gameObject.transform.localScale = new Vector3(15, 15, 15);
                tempPepper = Instantiate(pepper, pepperPos, pepperRot);
                agentsInScene += 1;
                distGroup = 50;

                // Draw line  or arrow around Pepper
                if (usingCircle == 1)
                {
                    float x;
                    float y = -0.8f;
                    float z;
                    float angle = 20f;
                    xradius = Random.Range(0.5f, 3f);
                    yradius = xradius;
                    robotRadius = xradius;
                    for (int i = 0; i < (segments + 1); i++)
                    {
                        x = pepperPos[0] + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                        z = pepperPos[2] + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
                        line.SetPosition(i, new Vector3(x, y, z));
                        angle += (360f / segments);
                    }
                }
                else
                {
                    // Find vector going out from pepper:
                    Vector3 tempArrowRot = tempPepper.gameObject.transform.forward;
                    Vector3 pos = pepperPos + (0.5f * tempArrowRot);
                    Quaternion arrowRot = tempPepper.gameObject.transform.rotation;
                    Vector3 temp_rot = pepperRot.eulerAngles;
                    temp_rot[0] -= 90;
                    temp_rot[1] += 90;
                    pos[1] = -0.8f;
                    arrowRot = Quaternion.Euler(temp_rot);
                    arrow.gameObject.transform.localScale = new Vector3(0.8f, 0.2f, 0.5f);
                    tempArrow = Instantiate(arrow, pos, arrowRot);

                    // Move circle out oof view
                    float x;
                    float y = -3f;
                    float z;
                    float angle = 20f;
                    xradius = Random.Range(0.5f, 3f);
                    yradius = xradius;
                    robotRadius = 0;
                    for (int i = 0; i < (segments + 1); i++)
                    {
                        x = pepperPos[0] + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                        z = pepperPos[2] + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
                        line.SetPosition(i, new Vector3(x, y, z));
                        angle += (360f / segments);
                    }
                }
            }

            nPeople = nPeopleGroup;

            // Now spawn the rest of the humans:
            // First normal people standing
            for (int i = nPeopleGroup; i < standingHumanCount; i++)
            {
                // If a group, avoid that area:
                humanArea = Random.Range(0, 4);
                if (nPeopleGroup > 0)
                {
                    while (humanArea == groupArea)
                    {
                        humanArea = Random.Range(0, 4);
                    }
                }

                (humanXPos, humanZPos) = SpawnPosition(humanArea);
                humanYPos = -0.921f;
                Vector3 humanPos = new Vector3(humanXPos, humanYPos, humanZPos);
                // Check if something's already there
                while (InvalidPosition(humanPos, temps, tempPepper.gameObject.transform.position))
                    {
                    (humanXPos, humanZPos) = SpawnPosition(humanArea);
                    humanYPos = -0.921f;
                    humanPos = new Vector3(humanXPos, humanYPos, humanZPos);
                }
                humanYRot = Random.Range(0, 360);
                Quaternion humanRot = Quaternion.Euler(0, humanYRot, 0);
                temps[i] = Instantiate(objects[i], humanPos, humanRot);
                agentsInScene += 1;
                instantatedStandingHumans += 1;
            }
            nPeople = standingHumanCount;

            // Spawn potential sofa people
            // Have people in sofa roughly every 5th scene:
            if (Random.Range(0, 5) == 1)
            {
                // 1 or 2 in the sofa
                nPeopleSofa = Random.Range(1, 3);
                // Spawn laying if only 1
                humanYRot = -89.62f;
                Vector3 humanPos = new Vector3(-5.7f, -0.29f, -0.27f);
                Quaternion humanRot = Quaternion.Euler(0, humanYRot, 0);
                objects[6].gameObject.transform.localScale = new Vector3(1, 1, 1);
                temps[6] = Instantiate(objects[6], humanPos, humanRot);
                agentsInScene += 1;
                if (nPeopleSofa > 1)
                {
                    humanYRot = -97.7f;
                    humanPos = new Vector3(-5.259f, -0.721f, -2.11f);
                    humanRot = Quaternion.Euler(0, humanYRot, 0);
                    objects[7].gameObject.transform.localScale = new Vector3(1, 1, 1);
                    temps[7] = Instantiate(objects[7], humanPos, humanRot);
                    agentsInScene += 1;
                }

            }
            else
            {
                nPeopleSofa = 0;
            }
            nPeople += nPeopleSofa;

            // Spawn potential children
            // Have children roughly every 5th scene:
            if (Random.Range(0, 5) == 1)
            {
                // Spawn first child
                childArea = Random.Range(0, 4);
                (childXPos, childZPos) = SpawnPosition(childArea);
                childYPos = -0.921f;
                Vector3 childPos = new Vector3(childXPos, childYPos, childZPos);
                // Check if something's already there
                while (InvalidPosition(childPos, temps, tempPepper.gameObject.transform.position))
                {
                    (childXPos, childZPos) = SpawnPosition(childArea);
                    childYPos = -0.921f;
                    childPos = new Vector3(childXPos, childYPos, childZPos);
                }
                childYRot = Random.Range(0, 360);
                Quaternion childRot = Quaternion.Euler(0, childYRot, 0);
                objects[8].gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                temps[8] = Instantiate(objects[8], childPos, childRot);
                nChildren += 1;
                agentsInScene += 1;
                distChildren = Vector3.Distance(childPos, tempPepper.gameObject.transform.position);
                // Have to childsren 50% of the time
                if (Random.Range(0,2) == 1)
                {
                    // Spawn second child
                    childArea = Random.Range(0, 4);
                    (childXPos, childZPos) = SpawnPosition(childArea);
                    childYPos = -0.921f;
                    childPos = new Vector3(childXPos, childYPos, childZPos);
                    // Check if something's already there
                    while (InvalidPosition(childPos, temps, tempPepper.gameObject.transform.position))
                    {
                        (childXPos, childZPos) = SpawnPosition(childArea);
                        childYPos = -0.921f;
                        childPos = new Vector3(childXPos, childYPos, childZPos);
                    }
                    childYRot = Random.Range(0, 360);
                    childRot = Quaternion.Euler(0, childYRot, 0);
                    objects[9].gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    temps[9] = Instantiate(objects[9], childPos, childRot);
                    agentsInScene += 1;
                    nChildren += 1;
                    if (Vector3.Distance(childPos, tempPepper.gameObject.transform.position) < distChildren)
                    {
                        distChildren = Vector3.Distance(childPos, tempPepper.gameObject.transform.position);
                    }
                }
            }
            else
            {
                distChildren = 50;
                nChildren = 0;
            }
            nPeople += nChildren;

            // Spawn potential animals
            // Have animals roughly every 5th scene:
            if (Random.Range(0, 5) == 1)
            {
                animalArea = Random.Range(0, 4);
                (animalXPos, animalZPos) = SpawnPosition(animalArea);
                animalYPos = -0.921f;
                Vector3 animalPos = new Vector3(animalXPos, animalYPos, animalZPos);
                // Check if something's already there
                while (InvalidPosition(animalPos, temps, tempPepper.gameObject.transform.position))
                {
                    (animalXPos, animalZPos) = SpawnPosition(animalArea);
                    animalYPos = -0.921f;
                    animalPos = new Vector3(animalXPos, animalYPos, animalZPos);
                }
                animalYRot = Random.Range(0, 360);
                Quaternion animalRot = Quaternion.Euler(0, animalYRot, 0);
                objects[10].gameObject.transform.localScale = new Vector3(70, 70, 70);
                temps[10] = Instantiate(objects[10], animalPos, animalRot);
                agentsInScene += 1;
                nAnimal = 1;
                distAnimal = Vector3.Distance(animalPos, tempPepper.gameObject.transform.position);
            }
            else
            {
                distAnimal = 50;
                nAnimal = 0;
            }

            // Find distance to the 3 closest humans:
            // Iterate over all standing humans, not children or animals!
            
            for (int i = 0; i < instantatedStandingHumans; i++)
            {
                Debug.Log(instantatedStandingHumans);
                Debug.Log(Vector3.Distance(temps[i].gameObject.transform.position, tempPepper.gameObject.transform.position));
                if (Vector3.Distance(temps[i].gameObject.transform.position, tempPepper.gameObject.transform.position) < distHuman3)
                {
                    if (Vector3.Distance(temps[i].gameObject.transform.position, tempPepper.gameObject.transform.position) < distHuman2)
                    {
                        if (Vector3.Distance(temps[i].gameObject.transform.position, tempPepper.gameObject.transform.position) < distHuman1)
                        {
                            // Update distance
                            distHuman3 = distHuman2;
                            distHuman2 = distHuman1;
                            directionHuman3 = directionHuman2;
                            directionHuman2 = directionHuman1;
                            distHuman1 = Vector3.Distance(temps[i].gameObject.transform.position, tempPepper.gameObject.transform.position);
                            closestHuman = i;
                            // Update direction
                            Vector3 closestHumanPos = temps[i].gameObject.transform.position;
                            Vector3 relativePos = closestHumanPos - tempPepper.gameObject.transform.position;
                            pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                            Vector3 temp_rot = pepperRot.eulerAngles - tempPepper.gameObject.transform.rotation.eulerAngles;
                            directionHuman1 = temp_rot[1];
                            // Find closest humans rotation as well
                            Quaternion closestHumanRot = Quaternion.LookRotation(-relativePos, Vector3.up);
                            temp_rot = closestHumanRot.eulerAngles - temps[i].gameObject.transform.rotation.eulerAngles;
                            directionRobotHuman1 = temp_rot[1];
                        }
                        else
                        {
                            // Update distance
                            distHuman3 = distHuman2;
                            directionHuman3 = directionHuman2;
                            distHuman2 = Vector3.Distance(temps[i].gameObject.transform.position, tempPepper.gameObject.transform.position);
                            closest2Human = i;
                            // Update direction
                            Vector3 closest2HumanPos = temps[i].gameObject.transform.position;
                            Vector3 relativePos = closest2HumanPos - tempPepper.gameObject.transform.position;
                            pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                            Vector3 temp_rot = pepperRot.eulerAngles - tempPepper.gameObject.transform.rotation.eulerAngles;
                            directionHuman2 = temp_rot[1];
                        }
                    }
                    else
                    {
                        // Update distance
                        distHuman3 = Vector3.Distance(temps[i].gameObject.transform.position, tempPepper.gameObject.transform.position);
                        closest3Human = i;
                        // Update direction
                        Vector3 closest3HumanPos = temps[i].gameObject.transform.position;
                        Vector3 relativePos = closest3HumanPos - tempPepper.gameObject.transform.position;
                        pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                        Vector3 temp_rot = pepperRot.eulerAngles - tempPepper.gameObject.transform.rotation.eulerAngles;
                        directionHuman3 = temp_rot[1];
                    }
                }
            }
            // Then iterate over children
            for (int i = 0; i < nChildren; i++)
            {
                Debug.Log(Vector3.Distance(temps[i+8].gameObject.transform.position, tempPepper.gameObject.transform.position));
                if (Vector3.Distance(temps[i+8].gameObject.transform.position, tempPepper.gameObject.transform.position) < distHuman3)
                {
                    if (Vector3.Distance(temps[i+8].gameObject.transform.position, tempPepper.gameObject.transform.position) < distHuman2)
                    {
                        if (Vector3.Distance(temps[i+8].gameObject.transform.position, tempPepper.gameObject.transform.position) < distHuman1)
                        {
                            distHuman3 = distHuman2;
                            distHuman2 = distHuman1;
                            directionHuman3 = directionHuman2;
                            directionHuman2 = directionHuman1;
                            distHuman1 = Vector3.Distance(temps[i+8].gameObject.transform.position, tempPepper.gameObject.transform.position);
                            closestHuman = i+8;
                            // Update direction
                            Vector3 closestHumanPos = temps[i+8].gameObject.transform.position;
                            Vector3 relativePos = closestHumanPos - tempPepper.gameObject.transform.position;
                            pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                            Vector3 temp_rot = pepperRot.eulerAngles - tempPepper.gameObject.transform.rotation.eulerAngles;
                            directionHuman1 = temp_rot[1];
                            // Find closest humans rotation as well
                            Quaternion closestHumanRot = Quaternion.LookRotation(-relativePos, Vector3.up);
                            temp_rot = closestHumanRot.eulerAngles - temps[i+8].gameObject.transform.rotation.eulerAngles;
                            directionRobotHuman1 = temp_rot[1];
                        }
                        else
                        {
                            distHuman3 = distHuman2;
                            directionHuman3 = directionHuman2;
                            distHuman2 = Vector3.Distance(temps[i+8].gameObject.transform.position, tempPepper.gameObject.transform.position);
                            closest2Human = i + 8;
                            // Update direction
                            Vector3 closest2HumanPos = temps[i + 8].gameObject.transform.position;
                            Vector3 relativePos = closest2HumanPos - tempPepper.gameObject.transform.position;
                            pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                            Vector3 temp_rot = pepperRot.eulerAngles - tempPepper.gameObject.transform.rotation.eulerAngles;
                            directionHuman2 = temp_rot[1];
                        }
                    }
                    else
                    {
                        distHuman3 = Vector3.Distance(temps[i+8].gameObject.transform.position, tempPepper.gameObject.transform.position);
                        closest3Human = i + 8;
                        // Update direction
                        Vector3 closest3HumanPos = temps[i + 8].gameObject.transform.position;
                        Vector3 relativePos = closest3HumanPos - tempPepper.gameObject.transform.position;
                        pepperRot = Quaternion.LookRotation(relativePos, Vector3.up);
                        Vector3 temp_rot = pepperRot.eulerAngles - tempPepper.gameObject.transform.rotation.eulerAngles;
                        directionHuman3 = temp_rot[1];
                    }
                }
            }

            // Make sure we only have positive angles
            if (directionHuman1 < 0)
            {
                directionHuman1 += 360;
            }
            if (directionHuman2 < 0)
            {
                directionHuman2 += 360;
            }
            if (directionHuman3 < 0)
            {
                directionHuman3 += 360;
            }
            if (directionRobotHuman1 < 0)
            {
                directionRobotHuman1 += 360;
            }

            // Determmine wether pepper is facing closest human
            if (Mathf.Abs(directionHuman1) < 45 || Mathf.Abs(directionHuman1) > 320)
            {
                robotFacingHuman1 = 1;
            }
            else if (robotWithinGroup == 1)
            {
                robotFacingHuman1 = 1;
            }
            else
            {
                robotFacingHuman1 = 0;
            }

            // Determine wether closest human is facing pepper  
            if (Mathf.Abs(directionRobotHuman1) < 45 || Mathf.Abs(directionRobotHuman1) > 320)
            {
                human1FacingRobot = 1;
            }
            else if (robotWithinGroup == 1)
            {
                human1FacingRobot = 1;
            }
            else
            {
                human1FacingRobot = 0;
            }
            

            // Add potential music
            if (Random.Range(0,10) == 1)
            {
                myText.text = "Music is playing";
                // Spawn laying if only 1
                Vector3 soundPos = new Vector3(-6.756f, 2.511f, -3.901f);
                Vector3 soundRot1 = new Vector3(0, 133, 0);
                Quaternion soundRot = Quaternion.Euler(soundRot1);
                sound.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                tempSound = Instantiate(sound, soundPos, soundRot);
                musicPlaying = 1;

            }
            else
            {
                myText.text = "";
                musicPlaying = 0;
            }
            // Construct filename:
            string filename = count.ToString() + "_" + usingCircle.ToString() + "_" + usingAarrow.ToString() + "_" +  nPeople.ToString() + "_" + nPeopleGroup.ToString() + "_" + groupRadius.ToString() + "_" + distGroup.ToString() + "_" + robotWithinGroup.ToString()
                + "_" + facingGroup.ToString() + "_" + robotRadius.ToString()  + "_" + distHuman1.ToString() + "_" + distHuman2.ToString() + "_" + distHuman3.ToString()
                + "_" + directionHuman1.ToString() + "_" + directionHuman2.ToString() + "_" + directionHuman3.ToString() + "_" + directionRobotHuman1.ToString() + "_" + robotFacingHuman1.ToString() + "_" + human1FacingRobot.ToString() + "_" + nChildren.ToString() + "_" + distChildren.ToString()
                + "_" + nAnimal.ToString() + "_" + nPeopleSofa.ToString() + "_" + agentsInScene.ToString();

            // Take screenshot and save to path
            ScreenCapture.CaptureScreenshot("data/screenshots/" + filename + ".png");
            string filePath = "data/screenshots/" + filename + ".png";
            // Write features to csv
            AppendToReport(new string[27] {
                count.ToString(),
                filePath,
                usingCircle.ToString(),
                usingAarrow.ToString(),
                nPeople.ToString(),
                nPeopleGroup.ToString(),
                groupRadius.ToString(),
                distGroup.ToString(),
                robotWithinGroup.ToString(),
                facingGroup.ToString(),
                robotRadius.ToString(),
                distHuman1.ToString(),
                distHuman2.ToString(),
                distHuman3.ToString(),
                directionHuman1.ToString(),
                directionHuman2.ToString(),
                directionHuman3.ToString(),
                directionRobotHuman1.ToString(),
                robotFacingHuman1.ToString(),
                human1FacingRobot.ToString(),
                nChildren.ToString(),
                distChildren.ToString(),
                nAnimal.ToString(),
                distAnimal.ToString(),
                nPeopleSofa.ToString(),
                musicPlaying.ToString(),
                agentsInScene.ToString() });


            // Remove all gameobjects
            // Pepper
            Destroy(tempPepper, 0.1f);
            // Arrow
            Destroy(tempArrow, 0.1f);
            // Sound
            Destroy(tempSound, 0.1f);

            for (int i = 0; i < 11; i++)
            {
                Destroy(temps[i], 0.1f);
            }

            yield return new WaitForSeconds(0.2f);
            count += 1;
        }

    }
}
