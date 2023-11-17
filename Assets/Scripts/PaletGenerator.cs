using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.ComponentModel;
using System;
using System.Threading;
using System.Text;
using TMPro;
using System.Globalization;
using JetBrains.Annotations;
using System.Drawing;
using UnityEditor;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;


public class PaletGenerator : MonoBehaviour
{
    //PaletGenCameraPos: 650, 70, -280
    //PaletOriginSpawn: 500,0,0

    public TMP_Text container;
    public TMP_Text skuOut;
    public TMP_Text weightOut;
    public TMP_Text lengthOut;
    public TMP_Text heightOut;
    public TMP_Text widthOut;
    public TMP_Text paletSizeOut;
    public TMP_Text paletWeightOut;
    public TMP_Text noContainerText;

    public GameObject serPref;
    public Transform serParent;
    public GameObject containerCam;


    public TMP_InputField contIDin;
    public TMP_InputField paletIDin;
    public TMP_InputField partIDin;
    public TMP_InputField serialIDin;

    public UnityWebRequest weightPHP;
    public UnityWebRequest lengthPHP;
    public UnityWebRequest widthPHP;
    public UnityWebRequest heightPHP;
    public UnityWebRequest qtyPHP;

    public float fweightKG;
    public float fwidthX;
    public float fheightY;
    public float flengthZ;
    public float fqty;
    public int c = 0;

    readonly string inputURL = "http://localhost/CoMS_Data/callInput.php";



    // Start is called before the first frame update
    void Start()
    {


    }

    public void contEnter() {

        if (contIDin.text == string.Empty) {

            container.text = "Invalid Container ID";

        }
        else {

            container.text = contIDin.text;
            Debug.Log(container.text);

        }

        paletIDin.ActivateInputField();



    }

    public void paletEnter() {

        if (paletIDin.text == string.Empty)
        {

            Debug.Log("Invalid entry");

        }
        else
        {

            Debug.Log(paletIDin.text);

        }

        partIDin.ActivateInputField();

    }
    public void partEnter() {

        if (partIDin.text == string.Empty)
        {

            Debug.Log("Invalid entry");

        }
        else
        {

            Debug.Log(partIDin.text);
            skuOut.text = partIDin.text;
            StartCoroutine(partGet(partIDin.text));
            StartCoroutine(getParams());
            StartCoroutine(convertParams());


        }

        serialIDin.ActivateInputField();
    }
    public void serialEnter() {

        if (serialIDin.text == string.Empty)
        {

            Debug.Log("Invalid entry");

        }
        else
        {

            Debug.Log(serialIDin.text);
            partSorter(fqty);

        }

        serialIDin.text = "";
        serialIDin.ActivateInputField();
    }

    IEnumerator partGet(string inDat) {

        Debug.Log("partGet method Executed");

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("partHook", inDat));

        UnityWebRequest www = UnityWebRequest.Post(inputURL, formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }




    }

    IEnumerator getParams() {


        weightPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callWeight.php");
        weightPHP.SendWebRequest();
        lengthPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callLength.php");
        lengthPHP.SendWebRequest();
        widthPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callWidth.php");
        widthPHP.SendWebRequest();
        heightPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callHeight.php");
        heightPHP.SendWebRequest();
        qtyPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callPartPerPallet.php");
        qtyPHP.SendWebRequest();


        yield return new WaitForSecondsRealtime(0.5f);

        weightOut.text = weightPHP.downloadHandler.text;
        lengthOut.text = lengthPHP.downloadHandler.text;
        widthOut.text = widthPHP.downloadHandler.text;
        heightOut.text = heightPHP.downloadHandler.text;

        Debug.Log(weightOut.text);
        Debug.Log(lengthOut.text);
        Debug.Log(widthOut.text);
        Debug.Log(heightOut.text);


    }

    IEnumerator convertParams() {



        yield return new WaitForSecondsRealtime(0.5f);

        bool xCheck = float.TryParse(widthOut.text, out float widthX);
        bool yCheck = float.TryParse(heightOut.text, out float heightY);
        bool zCheck = float.TryParse(lengthOut.text, out float lengthZ);
        bool wtCheck = float.TryParse(weightOut.text, out float weightKG);
        bool qtyCheck = float.TryParse(qtyPHP.downloadHandler.text, out float qtyPallet);

        fwidthX = widthX;
        fheightY = heightY;
        flengthZ = lengthZ;
        fweightKG = weightKG;
        fqty = qtyPallet;

        Debug.Log("Converted " + fwidthX + "cm");
        Debug.Log("Converted " + fheightY + "cm");
        Debug.Log("Converted " + flengthZ + "cm");
        Debug.Log("Converted " + fweightKG + "kg");
        Debug.Log("Quantity Per Pallet: " + fqty);




    }

    public void partSorter(float partPerPalet)
    {

        switch (partPerPalet)
        {

            case 6:

                sixPart();
                break;

        }


    }

    public void weightCalculator() { 
    
    
    
    
    
    }

    void sixPart() {



        var partNaming = new string[6];
        partNaming[0] = "item1";
        partNaming[1] = "item2";
        partNaming[2] = "item3";
        partNaming[3] = "item4";
        partNaming[4] = "item5";
        partNaming[5] = "item6";

        var ser6Naming = new string[6];
        ser6Naming[0] = "Serial1";
        ser6Naming[1] = "Serial2";
        ser6Naming[2] = "Serial3";
        ser6Naming[3] = "Serial4";
        ser6Naming[4] = "Serial5";
        ser6Naming[5] = "Serial6";



        switch (c) {

            case 0: //1st

                GameObject partSim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSim.transform.position = new Vector3((fwidthX/2) + 500, 0, 0);
                partSim.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSim.name = partNaming[c];

                break;

            case 1: //2nd

                GameObject l1Destroy = GameObject.Find("item1");
                Destroy(l1Destroy);

                GameObject partSiml1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSiml1.transform.position = new Vector3(500, 0, 0);
                partSiml1.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSiml1.name = partNaming[0];

                GameObject partSim2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSim2.transform.position = new Vector3(500 + fwidthX, 0, 0);
                partSim2.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSim2.name = partNaming[c];

                break;

            case 2: //3rd

                GameObject partSim3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSim3.transform.position = new Vector3((fwidthX / 2) + 500, fheightY, 0);
                partSim3.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ); 
                partSim3.name = partNaming[c];

                break;

            case 3: //4th

                GameObject l2Destroy = GameObject.Find("item3");
                Destroy(l2Destroy);

                GameObject partSiml2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSiml2.transform.position = new Vector3(500, fheightY, 0);
                partSiml2.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSiml2.name = partNaming[2];

                GameObject partSim4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSim4.transform.position = new Vector3(500 + fwidthX, fheightY, 0);
                partSim4.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSim4.name = partNaming[c];

                break;

            case 4: //5th

                GameObject partSim5 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSim5.transform.position = new Vector3((fwidthX / 2) + 500, fheightY * 2, 0);
                partSim5.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSim5.name = partNaming[c];

                break;

            case 5: //6th

                GameObject l3Destroy = GameObject.Find("item5");
                Destroy(l3Destroy);

                GameObject partSiml3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSiml3.transform.position = new Vector3(500, fheightY * 2, 0);
                partSiml3.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSiml3.name = partNaming[4];

                GameObject partSim6 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                partSim6.transform.position = new Vector3(500 + fwidthX, fheightY * 2, 0);
                partSim6.transform.localScale = new Vector3(fwidthX, fheightY, flengthZ);
                partSim6.name = partNaming[c];

                break;


        }

   

        var ser1 = Instantiate(serPref, serParent);
        ser1.GetComponent<TMP_Text>().text = serialIDin.text;
        ser1.name = ser6Naming[c] ;



        c++;
    }

    
    public void containerSel(int choiceIn) {

        switch (choiceIn) { 
        
            case 0:

                Debug.Log("Default");
                containerCam.transform.position = new Vector3(-8377,191.4234f,-1453.412f);
                noContainerText.text = "No Container Selected";
                break;
                

            case 1:

                //20Dry
                Debug.Log("Case 1");
                containerCam.transform.position = new Vector3(-1815f, 191.434f, -1453.25f);
                noContainerText.text = "";
                break;

            case 2:

                //40Dry
                containerCam.transform.position = new Vector3(-3955f, 191f, -1453.412f);
                noContainerText.text = "";
                break;

            case 3:

                //40Hi
                containerCam.transform.position = new Vector3(-6390f, 191.4234f, -1453.412f);
                noContainerText.text = "";
                break;

        
        }

        //Container Building horizon -3000, -137, 0
        //Container Camera 20Dry = -2037, -129, -418.25 rotation = 0.618, 3.633, 0.396
        //new rotation 14.223 5.605 1.254

    }


   



    public void testCode() {





    }
    



    // Update is called once per frame
    void Update()
    {


    }
}
