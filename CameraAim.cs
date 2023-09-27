using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class CameraAim : MonoBehaviour
{
   
    [SerializeField] Transform _player;
    Camera _cam;


    [Header("---------")]
    [Space(10)]
    
    [SerializeField] LayerMask _layerMask;
    
    [SerializeField]
    GameObject _oldGameObject, _currentGameObject;
    // Start is called before the first frame update
    void Start()
    {
       
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        
        Vector3 dir =  _cam.transform.position - _player.transform.position;
        Debug.DrawRay(_player.transform.position, dir*5,Color.red);
        RaycastHit hit;
        if(Physics.Raycast(_player.transform.position, dir.normalized,out hit, dir.sqrMagnitude, _layerMask))
        {
            if (hit.collider != null)
            {

                _currentGameObject = hit.collider.gameObject;
               
            }else
                _currentGameObject = null;
        }
        else
        {
            _currentGameObject = null;
        }


        if(_currentGameObject == null)
        {
            if (_oldGameObject != null)
                setMaterialAlpha(_oldGameObject, 1);
            return;
        }

        setMaterialAlpha(_currentGameObject, 0.5f);
        if (_currentGameObject.Equals(_oldGameObject))
            return;

        if(_oldGameObject != null)
            setMaterialAlpha(_oldGameObject, 1);

        _oldGameObject = _currentGameObject;
    }

   void setMaterialAlpha(GameObject g, float alpha)
    {
        List<Material> Mats = new List<Material>();
        Renderer r = g.GetComponent<Renderer>();
        if (r != null)
            Mats.AddRange(getMaterials(r));

        Renderer[] renderers = g.GetComponentsInChildren<Renderer>();
        foreach (Renderer _r in renderers)
        {
            Mats.AddRange(getMaterials(_r));
        }

        ChangeAlpha(Mats, alpha);
      
    }

     

    List<Material> getMaterials(Renderer r)
    {
        return r.materials.ToList();
    }

    void ChangeAlpha(List<Material> mats, float alphaVal)
    {
        foreach (Material mat in mats)
        {
            if (alphaVal == 1)
                mat.ToOpaqueMode();
            else
                mat.ToFadeMode();
            Color oldColor = mat.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);

            try
            {
                mat.SetColor("_Color", newColor);
            }
            catch (Exception e)
            {

            }
        }
       

    }
}
