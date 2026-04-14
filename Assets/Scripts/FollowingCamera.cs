using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private GameObject _character;
    [SerializeField] private float _returnSpeed;
    [SerializeField] private float _height;
    [SerializeField] private float _rearDistance;

    private Vector3 currentVector;

    public void Start()
    {
        _returnSpeed = 100f;
    }

    void Update()
    {
        if (_character == null)
        {
            var player = GameObject.FindGameObjectsWithTag("Player").Where(player => player.GetComponent<Player>().isOwned).ToList();
            if (player.Count() != 0)
            {
                _character = player[0];
                transform.rotation = Quaternion.Euler(75f, 0, 0);
            }
        }
        else
        {
            CameraMove();
        }
    }

    void CameraMove()
    {
        currentVector = new Vector3(_character.transform.position.x, _character.transform.position.y + _height, _character.transform.position.z - _rearDistance);
        transform.position = Vector3.Lerp(transform.position, currentVector, _returnSpeed * Time.deltaTime);
    }
}
