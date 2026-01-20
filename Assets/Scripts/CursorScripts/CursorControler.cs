using UnityEngine;

namespace SkellyCursor
{
    public class CursorControler : MonoBehaviour
    {
        public static CursorControler Instance { get; private set;}

        [SerializeField] private Texture2D cursorTextureDefault;
        [SerializeField] private Texture2D cursorTextureHover;
        [SerializeField] private Texture2D cursorTextureGrab;

        [SerializeField] private Vector2 clickPosition = Vector2.zero;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cursor.SetCursor(cursorTextureDefault, clickPosition, CursorMode.Auto);
        }
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                SetToMode(ModeOfCursor.Grab);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                SetToMode(ModeOfCursor.Default);
            }
        }
        public void SetToMode(ModeOfCursor modeOfCursor)
        {
            switch (modeOfCursor)
            {
                case ModeOfCursor.Default:
                    Cursor.SetCursor(cursorTextureDefault, clickPosition, CursorMode.Auto);
                    break;
                case ModeOfCursor.Hover:
                    Cursor.SetCursor(cursorTextureHover, clickPosition, CursorMode.Auto);
                    break;
                case ModeOfCursor.Grab:
                    Cursor.SetCursor(cursorTextureGrab, clickPosition, CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(cursorTextureDefault, clickPosition, CursorMode.Auto);
                    break;
            }
        }
    }
}