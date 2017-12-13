using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team {

	public string teamName; //the name of the team, one of the four corporations of the in-game lore
	public string teamColor; //the color associated with the corporations from the lore

	public int teamRanking; //the current ranking of the team, like what place in the competition

	public List<PlayerController> teamMembers; //what players are on the team

	public Team(string teamName, string teamColor, int teamRanking){
		this.teamName = teamName;
		this.teamColor = teamColor;
		this.teamRanking = teamRanking;
	}

}