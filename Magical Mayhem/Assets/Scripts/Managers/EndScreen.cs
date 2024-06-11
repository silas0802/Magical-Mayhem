using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public static EndScreen instance;

    [SerializeField] private Button leaveLobbyButton;

    private void Awake(){

        if (instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
        leaveLobbyButton.onClick.AddListener(LeaveButton);
    }
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LeaveButton(){
        SceneManager.LoadScene("Connection Screen");
    }
}
