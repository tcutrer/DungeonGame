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
        [SerializeField] private Vector2 cursorHotSpot;


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
            cursorHotSpot = new Vector2(80, 0);
            Cursor.SetCursor(cursorTextureDefault, cursorHotSpot, CursorMode.Auto);
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
            // On Mac, always use default cursor
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                return;
            }

            switch (modeOfCursor)
            {
                case ModeOfCursor.Default:
                    Cursor.SetCursor(cursorTextureDefault, cursorHotSpot, CursorMode.Auto);
                    break;
                case ModeOfCursor.Hover:
                    Cursor.SetCursor(cursorTextureHover, cursorHotSpot, CursorMode.Auto);
                    break;
                case ModeOfCursor.Grab:
                    Cursor.SetCursor(cursorTextureGrab, cursorHotSpot, CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(cursorTextureDefault, cursorHotSpot, CursorMode.Auto);
                    break;
            }
        }
    }
}