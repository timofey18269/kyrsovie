
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class main : MonoBehaviour
    {
        public float speed = 0.001f;
        public float awiting = 1;
        public GameObject myPrefab;
        GameObject newObject;
        int i = 0;

        void Start()
        {
            newObject = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newObject.name = "ban";
        }


        void Update()
        {
            
            while (i < 500) {
                if(i%10 == 0)
                {
                    newObject.transform.position += new Vector3(1f, 0f, 0f);
                    newObject.transform.Find("Znachnie").GetComponent<TextMeshPro>().text = (i + 1).ToString();
                }
                
                i ++;
            }
        }
    }
}