using UnityEngine;

namespace Gamekit3D
{
    public class OpenURL : MonoBehaviour
    {
        public string websiteAddress;

        public void OpenURLOnClick()
        {
            Application.OpenURL(websiteAddress);
        }
    } 
}