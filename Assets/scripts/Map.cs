﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour {

	public IntVector2 size;
	
	public MapCell cellPrefab;
	public Passage passagePrefab;
	public Wall wallPrefab;

	private MapCell[,] cells;

	public float generationStepDelay;

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

	public IEnumerator Generate_map () {
		WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
		cells = new MapCell[size.x, size.z];
		for (int x = 0; x < size.x; x++) {
			for (int z = 0; z < size.z; z++) {
				yield return delay;
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

		IntVector2 Center1 = new IntVector2(3,3);
		IntVector2 Center2 = new IntVector2(12,3);
		IntVector2 Center3 = new IntVector2(21,3);
		IntVector2 Center4 = new IntVector2(3,12);
		IntVector2 Center5 = new IntVector2(3,21);
		IntVector2 Center6 = new IntVector2(12,12);
		IntVector2 Center7 = new IntVector2(12,21);
		IntVector2 Center8 = new IntVector2(21,12);
		IntVector2 Center9 = new IntVector2(21,21);
		CreateRooms(Center1, 3);
		CreateRooms(Center2, 3);
		CreateRooms(Center3, 3);
		CreateRooms(Center4, 3);
		CreateRooms(Center5, 3);
		CreateRooms(Center6, 3);
		CreateRooms(Center7, 3);
		CreateRooms(Center8, 3);
		CreateRooms(Center9, 3);
	}
	
	private MapCell CreateCell (IntVector2 coordinates) {
		MapCell newCell = Instantiate(cellPrefab) as MapCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Map Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition =
			new Vector3(coordinates.x - size.x * 0.5f + 0.5f, -0.5f, coordinates.z - size.z * 0.5f + 0.5f);
		return newCell;
	}

	private void CreatePassage (MapCell cell, MapCell otherCell, CellDirection direction) {
		Passage passage = Instantiate(passagePrefab) as Passage;
		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(passagePrefab) as Passage;
		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}
	
	private void CreateWall (MapCell cell, MapCell otherCell, CellDirection direction) {
		if(cell == null) return;
		//create 2 walls
		Destroy(cell.GetEdge(direction));
		Wall wall = Instantiate(wallPrefab) as Wall;
		wall.Initialize(cell, otherCell, direction);
		wall.transform.localPosition +=
			new Vector3(0, 0.5f, 0);

		if (otherCell != null) {
			wall = Instantiate(wallPrefab) as Wall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
			wall.transform.localPosition +=
				new Vector3(0, 0.5f, 0);
		}

	}

	private void CreateRooms(IntVector2 center, int radius){
		int edge_cell_num = radius*2+1;
		int door_pos = UnityEngine.Random.Range(0, edge_cell_num * 4 - 4 - 1);
		bool door = false;
//		int door_pos = 16;
//		Debug.Log (door_pos);

		IntVector2 coord = center + (CellDirection.North.ToIntVector2() + CellDirection.West.ToIntVector2()) * radius;

		if(CoordMakeSense(coord)){
			MapCell NWCell = GetCell(coord);
			CreateRoomEdge(NWCell, CellDirection.North, edge_cell_num,ref door_pos, ref door);

		}

		coord = center +  (CellDirection.North.ToIntVector2() + CellDirection.East.ToIntVector2()) * radius;
		if(CoordMakeSense(coord)){
			MapCell NECell = GetCell(coord);
			CreateRoomEdge(NECell, CellDirection.East, edge_cell_num,ref door_pos, ref door);
		}

		coord = center +  (CellDirection.East.ToIntVector2() + CellDirection.South.ToIntVector2()) * radius;
		if(CoordMakeSense(coord)){
			MapCell ESCell = GetCell(coord);
			CreateRoomEdge(ESCell, CellDirection.South, edge_cell_num,ref door_pos, ref door);
		}

		coord = center +  (CellDirection.South.ToIntVector2() + CellDirection.West.ToIntVector2()) * radius;

		if(CoordMakeSense(coord)){
			MapCell SWCell = GetCell(coord);
			CreateRoomEdge(SWCell, CellDirection.West, edge_cell_num,ref door_pos, ref door);
		}

//		while(!door){
//			door_pos = UnityEngine.Random.Range(0, edge_cell_num * 4 - 4 - 1);
//			CreateRooms(center, radius);
//		}

	}

	private void CreateRoomEdge(MapCell startCell, CellDirection direction, int repitition, ref int door_pos, ref bool door){


//		IntVector2[] vectors = {
//			new IntVector2(1, 0),
//			new IntVector2(0, -1),
//			new IntVector2(-1, 0),
//			new IntVector2(0, 1)
//		};
//
//
//		for(int i = 0; i< repitition; i++,door_pos--){
//			MapCell neighbor = GetNeighbor(startCell, direction);
//			IntVector2 next = startCell.coordinates + vectors[(int)direction];
//			if(door_pos == 0) {
//				if(neighbor == null){
//					door_pos++;
//				}
//				else {
//					if(CoordMakeSense(next)) startCell = GetCell(next);
//					door = true;
//					continue;
//				}
//
//			}
//
//			CreateWall (startCell, neighbor, direction);
//
//			if(CoordMakeSense(next)) startCell = GetCell(next);
//
//		}
	}

}
