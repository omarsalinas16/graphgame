using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
using Model;
using UnityEngine.SceneManagement;
public class LevelsController : MonoBehaviour {


	public GameObject menuController;

	public GameObject canvas;

	public GameObject buttonLeft;
	public GameObject buttonRigth;
	private GameObject currentMenu;
	private List<GameObject> menus;

	public Transform buttonLevel;

	public Transform panelParent;

	[SerializeField]
	private float LEFT_MARGIN = 20.0f;
	private const float RIGTH_MARGIN = 20.0f;

	[SerializeField]
	private float TOP_MARGIN = 60.0f;
	[SerializeField]
	private float BOTTOM_MARGIN = 20.0f;

	[SerializeField]
	private float WIDTH_BUTTON = 200.0f;
	public const float SPACE_X = 30.0f;

	[SerializeField]
	private float HEIGHT_BUTTON = 80.0f;
	public const float SPACE_Y = 10.0f;
	public float DUMMY_NUM_BUTTONS = 20;
	
	private static string MAIN_SCENE_NAME = "Main";

	void Start() {
		addDinamicallyButtons();
	}

	public void leave() {
		MenuController menuControllerScript = menuController.GetComponent<MenuController>();
		menuControllerScript.appearForm(MenuController.Forms.MAINMENU);
	}

	public void addDinamicallyButtons() {
		RectTransform rectTransform = canvas.GetComponent<RectTransform>();
		float canvas_width = rectTransform.rect.width;
		float canvas_height = rectTransform.rect.height;
	
		float spaceForButtonsX = canvas_width - (LEFT_MARGIN + RIGTH_MARGIN);
		float numberButtonsX = (spaceForButtonsX - SPACE_X) / (WIDTH_BUTTON + SPACE_X);

		float widthButtons = WIDTH_BUTTON;
		// Check if has decimals in the first position
		string nButtonsWithDecimals = string.Format("{0:N1}", numberButtonsX);
		string nButtonsDecimals = nButtonsWithDecimals.Substring(nButtonsWithDecimals.LastIndexOf('.') + 1);
		numberButtonsX = (float)Math.Round(numberButtonsX);
		bool isCero = numberButtonsX == 0;
		if(isCero) {
			numberButtonsX = 1;
		}
		if(String.Compare(nButtonsDecimals, "0") != 0 || isCero) {
			widthButtons = (spaceForButtonsX - (numberButtonsX - 1) * SPACE_X) / numberButtonsX;
		}
		

		float spaceForButtonsY = canvas_height - (TOP_MARGIN + BOTTOM_MARGIN);
		float numberButtonsY = (spaceForButtonsY - SPACE_Y) / (HEIGHT_BUTTON + SPACE_Y);
		
		float heightButtons = HEIGHT_BUTTON;
		// Check if has decimals in the first position
		nButtonsWithDecimals = string.Format("{0:N1}", numberButtonsY);
		nButtonsDecimals = nButtonsWithDecimals.Substring(nButtonsWithDecimals.LastIndexOf('.') + 1);
		numberButtonsY = (float)Math.Round(numberButtonsY);
		isCero = numberButtonsY == 0;
		if(isCero) {
			numberButtonsY = 1;
		}
		if(String.Compare(nButtonsDecimals, "0") != 0 || isCero) {			
			heightButtons = (spaceForButtonsY - (numberButtonsY - 1) * SPACE_Y) / numberButtonsY;			
		}
		

		float[] positionsX = new float[(int)numberButtonsX];
		for(int i = 0; i < positionsX.Length; i++) {
			positionsX[i] = (LEFT_MARGIN + widthButtons/2 + i*SPACE_X + i*widthButtons) - (canvas_width / 2); 			
		}

		float[] positionsY = new float[(int)numberButtonsY];
		for(int i = 0; i < positionsY.Length; i++) {
			positionsY[i] = (canvas_height / 2) - (TOP_MARGIN + heightButtons/2 + i*SPACE_Y + i*heightButtons); 
		}
		

		int totalOfButtonThatFit = (int) (numberButtonsX * numberButtonsY);

		int countOfButtons = 0;

		// Getting the levels
		List<LevelLocal> levels = LevelsBuilder.GetAll();
		menus = new List<GameObject>();		
		do {
			GameObject menuTemp = new GameObject();
			menuTemp.name = "Menu " + (menus.Count + 1);
			menuTemp.transform.SetParent(panelParent, false);		
			for(int py = 0; py < numberButtonsY ; py++) {			
				bool getOut = false;			
				for(int px = 0; px < numberButtonsX ; px++) {								
					var buttonLevelInst = Instantiate(buttonLevel, menuTemp.transform, false);					
					buttonLevelInst.GetComponentInChildren<TextMeshProUGUI>().text = levels[countOfButtons].Name;
					//buttonLevelInst.GetComponentInChildren<Text>().text = "L " + (countOfButtons + 1);
					buttonLevelInst.gameObject.name = "ButtonLevel" + countOfButtons;								
					RectTransform buttonRect = buttonLevelInst.GetComponent<RectTransform>();
					buttonRect.localPosition = new Vector3(positionsX[px],positionsY[py],0);				
					buttonRect.sizeDelta = new Vector2(widthButtons, heightButtons);				
					Debug.Log("Id del nivel " + levels[countOfButtons].Id);
					int c = countOfButtons;
					buttonLevelInst.gameObject.GetComponent<Button>().onClick.AddListener(delegate {						
						AppearNewScene(levels[c]);																		
					});
					if(countOfButtons >= levels.Count()) {
						getOut = true;
						break;
					}
					countOfButtons++;
				}	
				if(getOut) {
					break;
				}			
			}
			menuTemp.SetActive(false);
			menus.Add(menuTemp);
		} while(countOfButtons < levels.Count());			
		currentMenu = menus[0];
		currentMenu.SetActive(true);
		if(menus.Count > 1) {			
			buttonRigth.SetActive(true);
		}
	}

	private void AppearNewScene(LevelLocal level) {
		LevelController.currentLevelIndex = level.Id;
		SceneManager.LoadScene(MAIN_SCENE_NAME);																		
	}

	public void GoRigth() {	
		currentMenu.SetActive(false);		
		int indexCurrentMenu = menus.FindIndex(m => m == currentMenu);
		currentMenu = menus[indexCurrentMenu + 1];
		currentMenu.SetActive(true);
		if(indexCurrentMenu + 2 >= menus.Count) {
			buttonRigth.SetActive(false);
		}
		buttonLeft.SetActive(true);
	}

	public void GoLeft() {	
		currentMenu.SetActive(false);		
		int indexCurrentMenu = menus.FindIndex(m => m == currentMenu);
		currentMenu = menus[indexCurrentMenu - 1];
		currentMenu.SetActive(true);
		if(indexCurrentMenu - 1 == 0) {
			buttonLeft.SetActive(false);
		}
		buttonRigth.SetActive(true);
	}

}
