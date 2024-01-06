using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveProgressManager
{

	public void Save()
	{

		Hashtable data = new Hashtable();

		for (int j = 0; j < (int) Shape.enumLength; j++)
		{

			string key = ((Shape) j).ToString();

			if (GameData.gameProperties.ContainsKey(key))
			{
				Hashtable field = new Hashtable();

				field.Add("score", GameData.gameProperties[key].score);

				field.Add("topScore", GameData.gameProperties[key].topScore);

				field.Add("undo", GameData.gameProperties[key].undoLevel);
				field.Add("hummer", GameData.gameProperties[key].hummerLevel);


				ArrayList cellsArray = new ArrayList();

				foreach (CellProperties cell in GameData.gameProperties[key].cells)
				{
					Hashtable cellProp = new Hashtable();
					cellProp.Add("i", cell.i);
					cellProp.Add("j", cell.j);
					cellProp.Add("v", cell.value);
					cellsArray.Add(cellProp);
				}

				ArrayList nextCellsArray = new ArrayList();

				foreach (int next in GameData.gameProperties[key].nextCells)
				{
					nextCellsArray.Add(next);
				}

				field.Add("cells", cellsArray);
				field.Add("next", nextCellsArray);

				data.Add(key, field);

			}
		}

		Write(data);
	}


	public void Decode(string pathToDecode)
	{

		if (File.Exists(pathToDecode))
		{
			string json = Read(pathToDecode);
			Hashtable data = Procurios.Public.JSON.JsonDecode(json) as Hashtable;
			if (data == null) return;

			foreach (DictionaryEntry de in data)
			{
				string key = de.Key.ToString();

				Hashtable thisfield = de.Value as Hashtable;
				SaveProgress thisGame = new SaveProgress();
				GameData.gameProperties.Add(key, thisGame);
				ParseField(thisfield, thisGame);
			}
		}
	}

	void ParseField(Hashtable field, SaveProgress thisGame)
	{

		foreach (DictionaryEntry de in field)
		{
			string key = de.Key.ToString();
			string value = de.Value.ToString();

			switch (key)
			{
				case "score":
					int val;
					if (int.TryParse(value, out val))
					{
						thisGame.score = val;
					}

					break;
				case "topScore":
					int topval;
					if (int.TryParse(value, out topval))
					{
						thisGame.topScore = topval;
					}

					break;
				case "undo":
					int undoval;
					if (int.TryParse(value, out undoval))
					{
						thisGame.undoLevel = undoval;
					}

					break;
				case "hummer":
					int hummerval;
					if (int.TryParse(value, out hummerval))
					{
						thisGame.hummerLevel = hummerval;
					}

					break;
				case "cells":
					ArrayList cells = de.Value as ArrayList;

					thisGame.cells = ParseCell(cells);
					break;
				case "next":
					ArrayList next = de.Value as ArrayList;

					thisGame.nextCells = ParseNextCells(next);
					break;
			}
		}
	}

	List<CellProperties> ParseCell(ArrayList cells)
	{

		List<CellProperties> list = new List<CellProperties>();
		foreach (object ce in cells)
		{
			Hashtable thisCell = ce as Hashtable;

			int i = 0;
			int j = 0;
			int v = 0;

			foreach (DictionaryEntry cellProp in thisCell)
			{
				string key = cellProp.Key.ToString();
				string value = cellProp.Value.ToString();

				switch (key)
				{
					case "i":
						int iVal = 0;
						if (int.TryParse(value, out iVal))
						{
							i = iVal;
						}

						break;
					case "j":
						int jVal = 0;
						if (int.TryParse(value, out jVal))
						{
							j = jVal;
						}

						break;

					case "v":
						int val = 0;
						if (int.TryParse(value, out val))
						{
							v = val;
						}

						break;
				}
			}

			CellProperties cell = new CellProperties(i, j, v);
			list.Add(cell);
		}

		return list;
	}

	List<int> ParseNextCells(ArrayList next)
	{
		List<int> list = new List<int>();

		foreach (object ne in next)
		{
			string value = ne.ToString();
			int val = 0;
			if (int.TryParse(value, out val))
			{
				list.Add(val);
			}
		}

		return list;
	}

	void Write(Hashtable data)
	{
		string jsonData = Procurios.Public.JSON.JsonEncode(data);

		if (File.Exists(GameConfig.path))
			File.Delete(GameConfig.path);

		File.WriteAllText(GameConfig.path, jsonData);
	}

	string Read(string pathToRead)
	{
		string jsonData = File.ReadAllText(pathToRead);
		return jsonData;
	}

	public void SaveTutorial(string tutorial, string path)
	{
		File.WriteAllText(path, tutorial);
	}
}

public class CellProperties
{

	public int i, j, value;

	public CellProperties(int i, int j, int value)
	{
		this.i = i;
		this.j = j;
		this.value = value;
	}
}