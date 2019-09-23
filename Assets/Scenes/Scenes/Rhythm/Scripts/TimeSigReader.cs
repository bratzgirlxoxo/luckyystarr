using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TimeSigReader : MonoBehaviour
{

  public GameObject notePrefab;
  public Transform notes;
  public string fileName;
  public AudioSource song;
  public int numStartBars;
  public float timeCounter;
  public float spawnY, hitY;
  public float lowX, middleX, highX;

  private StreamReader fileReader;

  private float bpm;
  private float timePerBar;
  private float distance;
  private float totalBarTime;
  private int baridx = 0;

  private float t;
  private float t2;
  private float nextNote;

  private bool songPlaying = false;
  private bool audioPlaying = false;

  private List<List<Note>> bars;

  void Start()
  {
    bars = GenerateSongData();
    PrepareSong(bars);
    Debug.Log("Playing at: " + bpm);
  }

  void Update()
  {
    t += Time.deltaTime;
    if (!songPlaying) {
      songPlaying = true;
      PlaySong(bars);
    }

    if (t >= timePerBar) {
      baridx++;
    }

    if (!audioPlaying && baridx == numStartBars) {
      song.Play();
    }
  }

  List<List<Note>> GenerateSongData() {
    string path = "Assets/Scenes/Scenes/Rhythm/Extras/" + fileName;
    string[] wholeFile = File.ReadAllLines(path);
    bpm = float.Parse(wholeFile[0]);

    List<List<Note>> bars = new List<List<Note>>();
    List<Note> bar1 = new List<Note>();
    List<Note> currBar = bar1;
    for (int i = 1; i < wholeFile.Length; i++) {
        if (wholeFile[i].Equals(",")) {
          bars.Add(currBar);
          currBar = new List<Note>();
          continue;
        }

        Note newNote = ParseNote(wholeFile[i]);
        currBar.Add(newNote);
    }

    timePerBar = (60f / bpm) * 4f;
    distance = spawnY - hitY;

    return bars;
  }

  void PrepareSong(List<List<Note>> allBars) {
    int barCounter = 0;

    for (int i = 0; i < allBars.Count; i++) {
      SpawnBar(i, spawnY + (i * distance));
    }
  }

  void PlaySong(List<List<Note>> allBars) {
    foreach (Transform t in notes) {
      t.GetComponent<NoteObject>().moving = true;
    }
  }

  void SpawnBar(int idx, float startY) {
    for (int i = 0; i < bars[idx].Count; i++) {

      float loc = startY + (distance / timeCounter) * bars[idx][i].location;

      if (bars[idx][i].low) {
        SpawnNote(lowX, loc);
      }
      if (bars[idx][i].middle) {
        SpawnNote(middleX, loc);
      }
      if (bars[idx][i].high) {
        SpawnNote(highX, loc);
      }
    }
  }

  void SpawnNote(float xPos, float yPos) {
    Vector3 spawnPos = new Vector3(xPos, yPos, 0f);
    GameObject newNote = Instantiate(notePrefab, spawnPos, Quaternion.identity);
    newNote.GetComponent<NoteObject>().speed = (distance / timePerBar);
    newNote.name = "Note";
    newNote.transform.parent = notes;
  }

  Note ParseNote(string noteData) {
    Note nNote = new Note();

    if (noteData[0].Equals('1')) {
      nNote.low = true;
    }
    if (noteData[1].Equals('1')) {
      nNote.middle = true;
    }
    if (noteData[2].Equals('1')) {
      nNote.high = true;
    }

    nNote.location = float.Parse(noteData[3].ToString());

    return nNote;
  }
}

// really basic class for each of the notes
// describes their tone and their location in the bar
class Note {
  public bool low;
  public bool middle;
  public bool high;
  public float location;

  public Note() {
    low = false;
    middle = false;
    high = false;
    location = 0f;
  }
}
