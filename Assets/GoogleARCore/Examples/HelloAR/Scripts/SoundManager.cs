using UnityEngine;

public class SoundManager {
	public static NormalizedAudioClip metalHit;
	public static NormalizedAudioClip swoosh;
	public static NormalizedAudioClip shatter;
	public static NormalizedAudioClip deathAwoken;
	public static NormalizedAudioClip rechambering;
	public static NormalizedAudioClip coinDropped;
	public static NormalizedAudioClip chestSlam;
	

	static SoundManager() {
		metalHit = new NormalizedAudioClip(Resources.Load<AudioClip>("Sounds/metal_hit"), 0.5f);
		swoosh = new NormalizedAudioClip(Resources.Load<AudioClip>("Sounds/swoosh"));
		shatter = new NormalizedAudioClip(Resources.Load<AudioClip>("Sounds/shatter"), 0.3f);
		deathAwoken = new NormalizedAudioClip(Resources.Load<AudioClip>("Sounds/demon_awoken"));
		rechambering = new NormalizedAudioClip(Resources.Load<AudioClip>("Sounds/rechambering"));
		coinDropped = new NormalizedAudioClip(Resources.Load<AudioClip>("Sounds/coin_dropped"));
		chestSlam = new NormalizedAudioClip(Resources.Load<AudioClip>("Sounds/chest_slam"));
	}
}

public class NormalizedAudioClip {

	private AudioClip clip;
	private float volume;
	
	public NormalizedAudioClip(AudioClip clip, float volume = 1f) {
		this.clip = clip;
		this.volume = volume;
	}
	
	public void Play() {
		PlaySingleSound.SpawnSound(clip, volume);
	}
}