using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _Speed = 3.0f;

    [SerializeField]
    private int _PowerupID;

    [SerializeField]
    private AudioClip _clip;

  
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _Speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);           

            if (player != null)
            {
                switch (_PowerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoPowerUpActive();
                        break;
                    case 4:
                        player.HealthPowerUpActive();
                        break;
                    case 5:
                        player.MultiShotActive();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }

            }
            Destroy(this.gameObject);
        }
    }

}
        