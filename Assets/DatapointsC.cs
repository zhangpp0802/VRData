using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
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
    private int[] thisRoundOutliers_update;
    private int thisRoundOutliers_current = 0;
    private int NumOfDataOnScreen = 100;
    private int MaxOutlierNum = 15;
    StreamReader strreader;
    private string fileLoc = "C:\\Users\\yzt8562\\Proj\\Assets\\covtype_w_outlier.csv";

    // Start is called before the first frame update
    void Start()
    {
        datalist = new GameObject[NumOfDataOnScreen];
        thisRoundOutliers = new int[MaxOutlierNum];
        thisRoundOutliers_update= new int[MaxOutlierNum];
        strreader = new StreamReader(fileLoc);
        ReadCSV();
    }

    void ReadCSV(){
        bool end = false;
        bool start = true;
        var i = 0;
        while(!end){
            string data = strreader.ReadLine();
            if(data == null){
                end = true;
                break;
            }
            if (i==NumOfDataOnScreen){
                end = true;
                break;
            }
            if (!start){
                var data_value = data.Split(",");
                var elevation = float.Parse(data_value[0]); // /1000
                var slope = float.Parse(data_value[1]);
                var cov_type = float.Parse(data_value[2]);

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(elevation, slope, cov_type);
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
        thisRoundOutliers_update=thisRoundOutliers;
        DetectOutlier();
        // if the outlier update, delete the outliers
        if (thisRoundOutliers_current!=thisRoundOutliers_update[0]){
            Debug.Log(thisRoundOutliers_update[0]);
            DeleteOutlier(thisRoundOutliers_update);
            thisRoundOutliers_current = thisRoundOutliers_update[0];
        }
        // set up for manually select
        for (int i = 0; i < datalist.Length; i++){
            GameObject cube = datalist[i];
            Rigidbody cubeRig = cube.GetComponent<Rigidbody>();
            XRGrabInteractable xrgrab = cube.GetComponent<XRGrabInteractable>();
            if (cubeRig.useGravity){
                cube.transform.position = new Vector3(0, 0, 0);
            }
        }
    }

    private float[] getInput_wasted(){
        StreamReader strreader = new StreamReader(fileLoc); 
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
            var elevation = float.Parse(data_value[0]);
            var slope = float.Parse(data_value[1]);
            var cov_type = float.Parse(data_value[2]);

            //create a cube
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(elevation, slope, cov_type);
            cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            datalist[currentInd] = cube;

            float[] returnfloat = new float[3];
            returnfloat[0] = float.Parse(data_value[0]);
            returnfloat[1] = float.Parse(data_value[1]);
            returnfloat[2] = float.Parse(data_value[2]);
            return returnfloat;
        }
        return null;
    }
    private float[] getInput(){
        string data = strreader.ReadLine();
        var data_value = data.Split(",");
        var elevation = float.Parse(data_value[0]); 
        var slope = float.Parse(data_value[1]);
        var cov_type = float.Parse(data_value[2]);
        float[] thisRoundData = new float[] { elevation, slope, cov_type };
        return thisRoundData;
    }

    private void DetectOutlier(){
        // Debug.Log(thisRoundOutliers_update[0]);
        float[] input = new float[3];
        input[0] = (float)1.0;
        input[1] = (float)1.0;
        input[2] = (float)1.0;
        if (initial){
            client.Predict(input, output =>
            {
                for (int x = 0; x < output.Length; x++) {
                    thisRoundOutliers[x] = output[x];
                }
            }, error =>
            {
                // TODO: when i am not lazy
            });
            // Application.Quit();
            initial = false;
        }
        else{
            input = getInput();
            // Debug.Log(input);
            if (input != null){
                client.Predict(input, output =>
                {
                    for (int x = 0; x < output.Length; x++) {
                    thisRoundOutliers[x] = output[x];
                }
                }, error =>
                {
                    // TODO: when i am not lazy
                });
            }
        }
    }

    private void DeleteOutlier(int[] output){
        int i = 0;
        for (int x = 0; x < output.Length; x++) {
            if (output[x]!=0){
                // Debug.Log("ino");
                int ind = output[x];
                GameObject cube = datalist[ind-i];
                Rigidbody cubeRig = cube.GetComponent<Rigidbody>();
                cube.transform.position = new Vector3(0, 0, 0);
                // Debug.Log(output[x]);
                i++;
            }
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
