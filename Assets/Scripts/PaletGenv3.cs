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
using System.Xml.Linq;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PaletGenv3 : MonoBehaviour
{

    public GameObject ContainerCam;
    public TMP_Text NoContainer;
    public TMP_InputField ContainerID_Input;
    public TMP_Text ContainerID_Output;
    public TMP_InputField PalletID_Input;
    public TMP_InputField SKU_Input;
    public TMP_Text SKU_Output;
    public TMP_InputField SerialID_Input;

    public UnityWebRequest weightPHP;
    public UnityWebRequest lengthPHP;
    public UnityWebRequest widthPHP;
    public UnityWebRequest heightPHP;
    public UnityWebRequest SKU_cbcmPHP;
    public TMP_Text weightOutput;
    public TMP_Text lengthOutput;
    public TMP_Text widthOutput;
    public TMP_Text heightOutput;
    public TMP_Text Total_CBM;
    public TMP_Text Total_Weight;
    public TMP_Text ContainerCapacity;
    public TMP_Text fillRate_Text;
    public TMP_Text totalContentWeight_Text;
    public UnityEngine.UI.Toggle palletToggler;

    public Transform SerialID_Parent;
    public GameObject SerialID_Template;
    public Transform PalletID_Parent;
    public Transform temporaryLocation;

    public float _20DRY_Capacity = 33;
    public float _40DRY_Capacity = 67;
    public float _40HC_Capacity = 76;

    float SKU_cbcm;
    float SKU_WeightKg;
    int part_Multiplier;
    float referenceX;
    float referenceY;
    float referenceZ;
    float referenceCBM;
    float referenceWeight;
    float loadingFactor;
    float fillRatio;
    float totalContentWeight;
    




    public void containerSel(int choiceIn)
    {

        switch (choiceIn)
        {

            case 0:

                
                ContainerCam.transform.position = new Vector3(-8377, 191.4234f, -1453.412f);
                NoContainer.text = "No Container Selected";
                ContainerCapacity.text = "0";
                break;


            case 1:

                //20Dry
                
                ContainerCam.transform.position = new Vector3(-1815f, 191.434f, -1453.25f);
                NoContainer.text = "";
                ContainerCapacity.text = _20DRY_Capacity.ToString();
                break;

            case 2:

                //40Dry
                ContainerCam.transform.position = new Vector3(-3955f, 191f, -1453.412f);
                NoContainer.text = "";
                ContainerCapacity.text = _40DRY_Capacity.ToString();
                break;

            case 3:

                //40Hi
                ContainerCam.transform.position = new Vector3(-6390f, 191.4234f, -1453.412f);
                NoContainer.text = "";
                ContainerCapacity.text = _40HC_Capacity.ToString();
                break;


        }

    }

    public void Container_Entry()
    {

        if (ContainerID_Input.text == string.Empty)
        {

            ContainerID_Output.text = "Invalid Container ID";

        }
        else
        {

            ContainerID_Output.text = ContainerID_Input.text;
            

        }

        if (Input.GetKeyDown(KeyCode.Return)) {


            PalletID_Input.ActivateInputField();


        }



    }

    public void PalletID_Entry()
    {

        if (PalletID_Input.text == string.Empty)
        {

            Debug.Log("Invalid entry");

        }
        else
        {

            Debug.Log(PalletID_Input.text);

        }
     
        if (Input.GetKeyDown(KeyCode.Return))
        {


            SKU_Input.ActivateInputField();


        }
    }

    public void SKU_Entry()
    {

        if (SKU_Input.text == string.Empty)
        {

            Debug.Log("Invalid entry");

        }
        else
        {

            Debug.Log(SKU_Input.text);
            SKU_Output.text = SKU_Input.text;
            StartCoroutine(partGet(SKU_Input.text));
            StartCoroutine(getParams());
            StartCoroutine(convertParams());

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {


            SerialID_Input.ActivateInputField();


        }
    }

    IEnumerator partGet(string inDat)
    {

        Debug.Log("partGet method Executed");

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("partHook", inDat));

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/CoMS_Data/callInput.php", formData);
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

    IEnumerator getParams()
    {

        weightPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callWeight.php");
        weightPHP.SendWebRequest();
        lengthPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callLength.php");
        lengthPHP.SendWebRequest();
        widthPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callWidth.php");
        widthPHP.SendWebRequest();
        heightPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callHeight.php");
        heightPHP.SendWebRequest();
        SKU_cbcmPHP = UnityWebRequest.Get("http://localhost/CoMS_Data/callCBCM.php");
        SKU_cbcmPHP.SendWebRequest();

        yield return new WaitForSecondsRealtime(2f);

        weightOutput.text = weightPHP.downloadHandler.text;
        lengthOutput.text = lengthPHP.downloadHandler.text;
        widthOutput.text = widthPHP.downloadHandler.text;
        heightOutput.text = heightPHP.downloadHandler.text;

    }

    IEnumerator convertParams()
    {

        yield return new WaitForSecondsRealtime(2f);

        float.TryParse(widthOutput.text, out float widthX);
        float.TryParse(heightOutput.text, out float heightY);
        float.TryParse(weightOutput.text, out float weightKG);
        float.TryParse(SKU_cbcmPHP.downloadHandler.text, out float SKU_Converted_cbcm);


        SKU_cbcm = SKU_Converted_cbcm;
        SKU_WeightKg = weightKG;
    }

    public void SerialID_Entry()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var serialget = Instantiate(SerialID_Template, SerialID_Parent);
            serialget.GetComponent<TMP_Text>().text = SerialID_Input.text;
            int childCount = SerialID_Parent.childCount;
            SerialID_Input.text = "";
            SerialID_Input.ActivateInputField();
            Weight_CBM_Calculator(childCount);

        }
        if (SerialID_Input.text == String.Empty) {

            Debug.Log("Invalid Serial");
        
        }


    }
    
    public void Weight_CBM_Calculator(int childValue)
    {

        part_Multiplier = childValue;
        float cbmInit = SKU_cbcm * childValue;
        float finalCBM = cbmInit / 1000000f;
        float finalWeight = SKU_WeightKg * childValue;

        Total_CBM.text = finalCBM.ToString();
        Total_Weight.text = finalWeight.ToString();

        referenceCBM = finalCBM;
        referenceWeight = finalWeight;

        Debug.Log("Current Pallet CBM:" + referenceCBM);
        Debug.Log("Current Pallet Weight" + referenceWeight);

    }

    public void Generate_PalletButton()
    {
       float reference_cbcm = SKU_cbcm * part_Multiplier;
       float colorPickerR = UnityEngine.Random.Range(0f, 1f);
       float colorPickerG = UnityEngine.Random.Range(0f, 1f);
       float colorPickerB = UnityEngine.Random.Range(0f, 1f);
        
       referenceX = Mathf.Pow((reference_cbcm), 1.0f / 3.0f);
       referenceY = Mathf.Pow((reference_cbcm), 1.0f / 3.0f);
       referenceZ = Mathf.Pow((reference_cbcm), 1.0f / 3.0f);

        GameObject current_Pallet = GameObject.CreatePrimitive(PrimitiveType.Cube);
        current_Pallet.transform.SetParent(PalletID_Parent, false);
        current_Pallet.transform.localScale = new Vector3(referenceX, referenceY, referenceZ);
        current_Pallet.transform.localPosition = new Vector3(0f, 0f, 0f);
        current_Pallet.GetComponent<Renderer>().material.color = new Color(colorPickerR,colorPickerG,colorPickerB,1);
        current_Pallet.name = PalletID_Input.text;



    }

    public void togglePallet()
    {

        if (palletToggler.isOn)
        {
            float yPos = -1f * ((referenceY / 2f) + (7.5f));
            Transform palletParent = GameObject.Find(PalletID_Input.text).transform;
            GameObject partPallet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            partPallet.transform.SetParent(PalletID_Parent, false);
            partPallet.transform.localScale = new Vector3(referenceX, 15f, referenceZ);
            partPallet.transform.localPosition = new Vector3(0f, yPos, 0f);
            partPallet.transform.SetParent(palletParent, true);
            partPallet.GetComponent<Renderer>().material.color = Color.yellow;

            float palletCBM = (referenceX * 15f * referenceZ) / 1000000f;
            float palletWeight = palletCBM * 5f;

            float palletizedCBM = referenceCBM + palletCBM;
            float palletizedWeight = referenceWeight + palletWeight;

            Total_CBM.text = palletizedCBM.ToString();
            Total_Weight.text = palletizedWeight.ToString();
        }
        else { Debug.Log("ToggleOff"); }

    }

    public void loadToContainer_Pressed(){

        if (PalletID_Parent.childCount == 0)
        {

            Debug.Log("No Pallet to load");

        }
        else {

            float.TryParse(Total_CBM.text, out float currentCBM);
            float.TryParse(ContainerCapacity.text, out float currentContainerCBM);

            loadingFactor = (currentCBM / currentContainerCBM) * 100;
            fillRatio = loadingFactor * 100 / currentContainerCBM;
            float.TryParse(fillRate_Text.text, out float currentFillRate);
            float.TryParse(totalContentWeight_Text.text, out float currentTotalContentWeight);
            float.TryParse(Total_Weight.text, out float currentTotalWeight);
            Debug.Log("Loading Factor:" + loadingFactor);
            Debug.Log("FillRatio:" + fillRatio);
            float updatedTotalContentWeight = currentTotalContentWeight + currentTotalWeight;
            float updatedFillRate = currentFillRate + fillRatio;

            fillRate_Text.text = updatedFillRate.ToString();
            totalContentWeight_Text.text = updatedTotalContentWeight.ToString();

            GameObject tobeLocated = GameObject.Find(PalletID_Input.text);
            tobeLocated.transform.SetParent(temporaryLocation, false);
            tobeLocated.transform.localPosition = new Vector3(-1655f, 4.5f, 55f);

            PalletID_Input.text = "";
            SKU_Input.text = "";
            SKU_Output.text = "SKU";
            weightOutput.text = "0";
            lengthOutput.text = "0";
            widthOutput.text = "0";
            heightOutput.text = "0";
            Total_CBM.text = "0";
            Total_Weight.text = "0";



            while (SerialID_Parent.childCount > 0)
            {
                DestroyImmediate(SerialID_Parent.GetChild(0).gameObject);
            }

            if (palletToggler.isOn)
            {
                palletToggler.isOn = false;
            }

            PalletID_Input.ActivateInputField();

        }


    }

    public void clearContents() {

        while (SerialID_Parent.childCount > 0)
        {
            DestroyImmediate(SerialID_Parent.GetChild(0).gameObject);
        }

        Total_CBM.text = "0";
        Total_Weight.text = "0";

    }





}
