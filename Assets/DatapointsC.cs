using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using UnityEditor.Scripting.Python;


public class DatapointsC : MonoBehaviour
{
    // initializing data list
    public GameObject[] datalist;
    // initializing client
    public DataClient client;
    public int lastInd;
    public int currentInd;
    private bool initial = true;
    private int[] thisRoundOutliers;

    // Start is called before the first frame update
    void Start()
    {
        datalist = new GameObject[31];
        ReadCSV();
        client = new DataClient();
        client.InitializeServer();
    }

    void ReadCSV(){
        StreamReader strreader = new StreamReader("C:\\Users\\yzt8562\\Proj\\Assets\\Salary_dataset.csv");
        bool end = false;
        bool start = true;
        var i = 0;
        while(!end){
            string data = strreader.ReadLine();
            if(data == null){
                end = true;
                break;
            }
            if (!start){
                var data_value = data.Split(",");
                var year = float.Parse(data_value[1]);
                var salary = float.Parse(data_value[2])/10000;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(year, salary, 0);
                cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                datalist[i] =cube;
                i++;

                // set up for manually select
                Rigidbody cubeRig = cube.AddComponent<Rigidbody>();
                cubeRig.useGravity = false;
                XRGrabInteractable xrgrab = cube.AddComponent<XRGrabInteractable>();
                xrgrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
                xrgrab.smoothPosition = true;
                xrgrab.forceGravityOnDetach = true;
            }
            else{
                start = false;
            }
        }
        lastInd = i;
        currentInd = i;
    }

    // Update is called once per frame 1/72
    void Update()
    {
        DetectOutlier();
        DeleteOutlier(thisRoundOutliers);

        // set up for manually select
        for (int i = 0; i < datalist.Length; i++){
            GameObject cube = datalist[i];
            Rigidbody cubeRig = cube.GetComponent<Rigidbody>();
            XRGrabInteractable xrgrab = cube.GetComponent<XRGrabInteractable>();
            if (cubeRig.useGravity){
                cube.transform.position = new Vector3(0, 0, 0);
            }
        }
        // FileRunnerFromPython();
    }

    private float[] getInput(){
        StreamReader strreader = new StreamReader("C:\\Users\\yzt8562\\Proj\\Assets\\Salary_dataset.csv"); 
        int i = 0;
        bool end = false;
        string last = "";
        while(!end){
            string data = strreader.ReadLine();
            if(data == null){
                end = true;
                break;
            }
            else{
                i++;
                last = data;
            }
        }   
        currentInd = i-1;

        if (currentInd>lastInd){
            var data = last;
            var data_value = data.Split(",");
            var year = float.Parse(data_value[1]);
            var salary = float.Parse(data_value[2])/10000;

            //create a cube
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(year, salary, 0);
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            datalist[currentInd] = cube;

            float[] returnfloat = new float[2];
            returnfloat[0] = float.Parse(data_value[1]);
            returnfloat[1] = float.Parse(data_value[2]);
            return returnfloat;
        }
        return null;
    }

    private void DetectOutlier(){
        float[] input = new float[2];
        input[0] = (float)1.0;
        input[1] = (float)1.0;
        if (initial){
            client.Predict(input, output =>
            {
                thisRoundOutliers = output;
            }, error =>
            {
                // TODO: when i am not lazy
            });
            // Application.Quit();
            initial = false;
        }
        // else{
        //     input = getInput();
        //     Debug.Log(input);
        //     if (input != null){
        //         client.Predict(input, output =>
        //         {
        //             thisRoundOutliers = output;
        //         }, error =>
        //         {
        //             // TODO: when i am not lazy
        //         });
        //     }
        // }
    }

    private void DeleteOutlier(int[] output){
        for (int x = 0; x < output.Length; x++) {
            // Debug.Log(output[x]);
            GameObject cube = datalist[output[x]];
            Rigidbody cubeRig = cube.GetComponent<Rigidbody>();
            cube.transform.position = new Vector3(0, 0, 0);
        } 
    }
    void StringRunnerFromPython()
    {
        PythonRunner.RunString(@"
                import UnityEngine;
                UnityEngine.Debug.Log('hello world')
                ");
    }

    void FileRunnerFromPython()
    {
        // string dp = "Assets/PythonFiles/test.py";
        string dp = "Assets/PythonFiles/test.py";
        PythonRunner.RunFile(dp);
    }
}
