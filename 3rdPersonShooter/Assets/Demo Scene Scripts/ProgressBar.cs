using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	
	public HealthController healthController;
	public Texture foregroundTexture;
	public Texture backgroundTexture;
	public Texture2D damageTexture;
	public bool rightSide = false;
	
	void OnGUI () {
		
		// Make rect 10 pixels from side edge and 6 pixels from top. 
		Rect rect = new Rect(10, 6, Screen.width/2-10-40, backgroundTexture.height);
		
		// If this is a right side health bar, flip the rect.
		if (rightSide) {
			rect.x = Screen.width - rect.x;
			rect.width = -rect.width;
		}
		
		// Draw the background texture
		GUI.DrawTexture(rect, backgroundTexture);
		
		float health = healthController.normalizedHealth;
		
		// Multiply width with health before drawing the foreground texture
		rect.width *= health;
		
		// Get color from damage texture
		//GUI.color = damageTexture.GetPixelBilinear(health, 0.5f);
		
		// Draw the foreground texture
		GUI.DrawTexture(rect, foregroundTexture);
		
		// Reset GUI color.
		GUI.color = Color.white;
	}
	
}















