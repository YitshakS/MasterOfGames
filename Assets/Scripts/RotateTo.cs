// מחלקה זו מסובבת אוביקט לעבר אוביקט במהירות הניתנת לשליטה
// למשל מבט של דמות לעבר אוביקט אותו היא מרימה, או מבט של דמות לעבר דמות שאיתה היא מקיימת אינטראקציה
// המבצע זאת מיידית LookAt במקום להשתמש ב

using UnityEngine;

public class RotateTo : MonoBehaviour
{
    public GameObject rotateObject; // אוביקט שיש לסובב
    public GameObject rotateToObject; // אוביקט שאליו יש לבצע את הסיבוב
    public float rotateObjectSpeed = 10f; // מהירות הסיבוב

    void Update()
    {
        if (rotateObject && rotateToObject)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotateToObject.transform.position - rotateObject.transform.position);

            // אם עדיין צריך לסובב
            if (Mathf.Abs(rotateObject.transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) > 1)
            {
                // ביצוע הסיבוב
                rotateObject.transform.eulerAngles = new Vector3(
                    rotateObject.transform.eulerAngles.x,
                    Quaternion.Slerp(rotateObject.transform.rotation, targetRotation, Time.deltaTime * rotateObjectSpeed).eulerAngles.y,
                    rotateObject.transform.eulerAngles.z
                );
            }
            else
            {
            //  rotateObject.transform.LookAt(rotateToObject.transform); // הסטת מבט הדמות לעבר האוביקט
                rotateObject = null;
                rotateToObject = null;
            }
        }
    }
}