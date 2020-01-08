using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
class demo : MonoBehaviour
{
    public int count;
    // Initialize all game objects:
    public GameObject pepper;
    public GameObject human1;
    public GameObject human2;
    public GameObject human3;
    public GameObject human4;
    public GameObject human5;
    public GameObject human6;
    public GameObject layingHuman;
    public GameObject phoneHuman;
    //public Animation phone;
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
    GameObject tempPhoneHuman;
    GameObject tempDog;
    public int human_count;

    // Position variables:
    // Pepper
    public float pepperXPos;
    public float pepperYPos;
    public float pepperZPos;
    public float pepperYRot;
    // 3 humans
    private float human1XPos;
    private float human1YPos;
    private float human1ZPos;
    private float human1YRot;
    private float human2XPos;
    private float human2YPos;
    private float human2ZPos;
    private float human2YRot;
    private float human3XPos;
    private float human3YPos;
    private float human3ZPos;
    private float human3YRot;
    // Animal
    private float animalXPos;
    private float animalYPos;
    private float animalZPos;
    private float animalYRot;
    // Children
    private float childXPos;
    private float childlYPos;
    private float childZPos;
    private float childRot;
    public int area;

    // Variables for circle around Pepper
    [Range(0, 50)]
    int segments = 50;
    [Range(0, 1)]
    public float xradius;
    [Range(0, 1)]
    public float yradius;
    LineRenderer line;

    // CSV file for storing of features and screenshots
    private static string reportFileName = "features.csv";
    private static string reportSeparator = ",";
    private static string[] reportHeaders = new string[16] {
        "Number of people",
        "Group of people?",
        "Distance to group",
        "Robot within group?"
        "Robot facing group?",
        "Distance to closest human",
        "Distance to 2nd closest human",
        "Distance to 3rd closest human",
        "Facing closest human?",
        "Number of children",
        "Distance to closest child",
        "Number of animals",
        "Distance to closest animal",
        "Anyone taking a phone call?",
        "Number of people sitting/laying in sofa?",
        "Stamp"
    };

    // Feauture Variables
    // For binary values 0 = False, 1 = True
    public int nPeople;
    public int group;  // Binary
    public float distGroup; // Euclidean distance to group, if no group -> 50
    public int robotWithinGroup;  // Binary
    public int facingGroup;  // Binary, if no group -> 0
    public float distHuman1; // Euclidean distance to closest human, if no people -> 50
    public float distHuman2; // Euclidean distance to 2nd closest human, if no people -> 50
    public float distHuman3; // Euclidean distance to 3d closest human, if no people -> 50
    public int facingHuman1;  // Binary
    public int nChildren;
    public float distChildren; // Euclidean distance to closest child, if no child -> 50
    public int nAnimal;
    public float distAnimal; // Euclidean distance to closest animal, if no animal -> 50
    public int phoneCall;  // Binary
    public int nPeopleSofa;


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
        List<GameObject> objects = new List<GameObject>() {human1, human2, human3, human4, human5, human6, layingHuman, phoneHuman, dog};
        List<GameObject> temps = new List<GameObject>() {tempHuman1, tempHuman2, tempHuman3, tempHuman4, tempHuman5, tempHuman6, tempLayingHuman, tempPhoneHuman, tempDog };
        // Attach animation
        //phone = phoneHuman.gameObject.GetComponent<Animation>();
        // Create csv file and initiate headers:
        CreateReport();
        // Start program
        StartCoroutine(SpawnRandom(objects, temps));
    }

    private (float, float) spawnPosition(int room_area)
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

    private static void AppendToReport(string[] strings)
    {
        using (StreamWriter sw = File.AppendText("Assets/data/features.csv"))
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
        using (StreamWriter sw = File.CreateText("Assets/data/features.csv"))
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

    IEnumerator SpawnRandom(List<GameObject> objects, List<GameObject> temps)
    {
        while (count < 10)
        {
            // Determine if group or not
            group = Random.Range(0, 1);


            // Instantiate Pepper
            // Define spawn position
            area = Random.Range(0, 4);
            (pepperXPos, pepperZPos) = spawnPosition(area);
            pepperYPos = -0.921f;
            pepperYRot = Random.Range(0, 360);
            Vector3 pepperPos = new Vector3(pepperXPos, pepperYPos, pepperZPos);
            Quaternion pepperRot = Quaternion.Euler(0, pepperYRot, 0);
            pepper.gameObject.transform.localScale = new Vector3(15, 15, 15);
            tempPepper = Instantiate(pepper, pepperPos, pepperRot);
            // Draw line around Pepper
            float x;
            float y = -0.8f;
            float z;
            float angle = 20f;
            xradius = Random.Range(0.5f, 3f);
            yradius = xradius;
            for (int i = 0; i < (segments + 1); i++)
            {
                x = pepperXPos + Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
                z = pepperZPos + Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
                line.SetPosition(i, new Vector3(x, y, z));
                angle += (360f / segments);
            }

            // Spawn humans
            // Get number of humans to spawn
            human_count = Random.Range(0, 7);
            for (int i = 0; i < human_count; i++)
            {
                area = Random.Range(0, 4);
                (humanXPos, humanZPos) = spawnPosition(area);
                humanYPos = -0.921f;
                humanYRot = Random.Range(0, 360);
                Vector3 humanPos = new Vector3(humanXPos, humanYPos, humanZPos);
                Quaternion humanRot = Quaternion.Euler(0, humanYRot, 0);
                temps[i] = Instantiate(objects[i], humanPos, humanRot);
            }

            // Take screenshot and save to path
            ScreenCapture.CaptureScreenshot("Assets/data/screenshots/context_" + count.ToString() + ".png");
            // Write features to csv
            AppendToReport(new string[5] {
                5.ToString(),
                1.ToString(),
                3.ToString(),
                1.ToString(),
                count.ToString() });
            // Remove all gameobjects
            Destroy(tempPepper, 0.1f);
            for (int i = 0; i < human_count; i++)
            {
                Destroy(temps[i], 0.1f);
            }
            yield return new WaitForSeconds(0.2f);
            count += 1;
        }

    }
}

