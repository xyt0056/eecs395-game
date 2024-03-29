﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;


public class Map : MonoBehaviour {
	
	public IntVector2 size;
	
	public MapCell cellPrefab;
	public Passage passagePrefab;
	public Wall wallPrefab;
	public Desk deskPrefab;
	public Colleague matePrefab;
	public Material roomfloormat;

	private MapCell[,] cells;
	
	public MapCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}
	
	private MapCell GetNeighbor(MapCell cell, CellDirection direction){
		MapCell neighbor = null;
		IntVector2 coordinates = cell.coordinates + direction.ToIntVector2();
		if(CoordMakeSense(coordinates)) neighbor = GetCell(coordinates);
		return neighbor;
	}
	
	private bool CoordMakeSense(IntVector2 coordinates){
		return (coordinates.x >=0 && coordinates.z >=0 
		        && coordinates.x <size.x && coordinates.z < size.z);
		
	}
	
	public void Generate_floor () {
		cells = new MapCell[size.x, size.z];
		for (int x = 0; x < size.x; x++) {
			for (int z = 0; z < size.z; z++) {
				CreateCell(new IntVector2(x, z));

				MapCell currentCell = cells[x,z];
				
				foreach(CellDirection direction in Enum.GetValues(typeof(CellDirection))){

					MapCell neighbor = GetNeighbor(currentCell, direction);
					if(neighbor != null) {
						CreatePassage(currentCell, neighbor, direction);
					}
				}
			}
		}
	}
	
	private MapCell CreateCell (IntVector2 coordinates) {
		MapCell newCell = Instantiate(cellPrefab) as MapCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition =
			new Vector3(coordinates.x * 2 - size.x, -0.5f, coordinates.z * 2 - size.z );
//		newCell.gameObject.layer = LayerMask.NameToLayer("Ground");
		return newCell;
	}
	
	private void CreatePassage (MapCell cell, MapCell otherCell, CellDirection direction) {
		if(cell == null) return;

		Passage passage = Instantiate(passagePrefab) as Passage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as Passage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}
	
	private void CreateWall (MapCell cell, MapCell otherCell, CellDirection direction) {
		if(cell == null) return;
		//create 2 walls
//		cell.GetEdge(direction).dest();

		Wall wall = Instantiate(wallPrefab) as Wall;
		wall.Initialize(cell, otherCell, direction);
		wall.transform.localPosition +=
			new Vector3(0, 1, 0);

//		wall.gameObject.layer = LayerMask.NameToLayer ("Obstacles");

		if (otherCell != null) {
//			otherCell.GetEdge(direction.GetOpposite()).dest();
			wall = Instantiate(wallPrefab) as Wall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
			wall.transform.localPosition +=
				new Vector3(0, 1, 0);
		}
		
	}

	private void CreateDesk (MapCell cell, MapCell otherCell, CellDirection direction) {
		if(cell == null) return;
		Desk desk = Instantiate(deskPrefab) as Desk;
//		desk.Initialize(cell, otherCell, direction);
		desk.transform.localPosition =
			cell.transform.localPosition+ new Vector3(0, 1,0);
		cell.hasdesk = true;
		desk.transform.parent = cell.transform;

		if(!otherCell.hasdesk){
			Colleague colleague = Instantiate(matePrefab) as Colleague;
			colleague.transform.localRotation = direction.ToRotation();
			colleague.transform.localPosition = cell.transform.localPosition+direction.ToVector3();
			colleague.transform.parent = desk.transform;

		} 
	}

	private void CreateEdge(MapCell cell, CellDirection direction, string wall, bool room){
		MapCell neighbor = GetNeighbor(cell, direction);
		if(wall == "1") CreateWall(cell, neighbor, direction);
		if(wall == "2") CreateDesk(cell, neighbor, direction);
		if(room){
			cell.transform.GetChild(0).GetComponent<Renderer>().material = roomfloormat;
		}
	}


	public bool load_map_from_file(string fileName){
		try{
			StreamReader theReader = new StreamReader(fileName, Encoding.Default);
			bool room = false;
			using (theReader){
				string line = null;
				do{
					line = theReader.ReadLine();
//					Debug.Log(line);
					if(line.StartsWith("//")) {
						if(line.EndsWith("rooms")){
							room = true;
						}
						continue;
					}
					string[] entries = line.Split(' ');
					if(entries.Length > 0) Create_map(entries, room);
				}
				while(line != null);
				theReader.Close();
				return true;
			}
		}
		catch(Exception){
//			Debug.Log("wrong reading file");
			return false;

		// From: boss-ai branch
		//		if(!desk){
		//			desk_pos = UnityEngine.Random.Range(0, edge_cell_num * 4 - 4 - 1);
		//			CreateRooms(center, radius);
		}
	}

	private void Create_map(string[] entries, bool room){
		if(entries[0] == "#") {
			Int32.TryParse(entries[1], out size.x);
			Int32.TryParse(entries[2], out size.z);
			Generate_floor();
			return;
		}

		else {
			IntVector2 coord = new IntVector2(-1,-1);
			Int32.TryParse(entries[0], out coord.x);
			Int32.TryParse(entries[1], out coord.z);

			CellDirection[] dirs = {CellDirection.West, CellDirection.North, CellDirection.East,CellDirection.South};
			for(int i = 0; i < 4;i++){
				CreateEdge(GetCell(coord),dirs[i],entries[i+2], room);
			}
		}
	}
	
}
