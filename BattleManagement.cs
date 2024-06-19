using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleManagement : MonoBehaviour
{
    #region Targeting Stuff
    private Board board;
    private int? enemyTargetSelected;
    private int? heroTargetSelectedToHelp;
    private int? heroTargetSelected;
    private UtilityTools utilityTools;
    #endregion

    #region EnemyStuff
    [Header("                                    Enemy Stuff")]
    private LevelManager levelManager;
    public List<Enemy> enemyList;
    private Enemy enemy1;
    private Enemy enemy2;
    private Enemy enemy3;
    public List<Image> imgEnemyBattleList;
    public List<Text> txtLifeEnemys;
    private EnemyBars enemyBars;
    #endregion

    #region Hero Stuff
    private HeroesManager heroesManager;
    private Hero heroBase;
    private Hero newHero;
    [HideInInspector]
    public List<Hero> heroList;
    [Header("                                    Hero Stuff")]
    public List<Image> imgHeroBattleList;
    public List<Image> imgHeroDotSkillStatus;
    public List<Text> txtLifeHeroes;
    private HeroBars heroBars;
    #endregion

    #region BattleItems Stuff
    [Header("                                    BattleItems Stuff")]
    public List<Button> btnBattleItemsList;
    public List<Text> itemQuantity;
    public List<Text> txtItemInfo;
    private InventoryManager inventory;
    private int? nItemSelected; //Número do item à ser usado
    #endregion 

    #region Calc Stuff
    private int dotCount;
    private List<int> nTargetsList;
    private int nThreat;
    private int randomTarget;
    private int randomDamage;
    private bool isAtkBlockedOrEvaded;
    private System.Random randomN;
    private int calcBuff;
    #endregion

    #region BattleDetails Stuff
    [Header("                                    Battle Details Stuff")]
    public List<Image> bkgImageList;
    public List<Image> imgMoldureSelectList;
    public List<Image> imgShadowMoldureSelectList;
    public List<Text> txtHeroAtkList;
    public List<Text> txtHeroLifeList;
    public List<Text> txtResourceList;
    public List<Text> txtHeroDefList;
    public List<Text> txtHeroNameList;
    public List<Image> imgIconSkillList;
    public List<Image> imgIconMoldureList;
    public List<Image> bkgSkillNameList;
    public List<Text> txtSkillDesList;
    public List<Text> txtSkillNameList;
    #endregion

    #region Animation Stuff
    private float waitTimebeforeMove;
    [Header("                                    Animation Stuff")]
    public GameObject floatingTextPrefab; //Float Dano/Heal/Block/Evasion/Defense
    public GameObject iconHeroAtkPrefab; //icones de indicação de ataque dos heróis
    public GameObject damageEffectPrefab; //Efeito de corte ao dar dano
    public List<ParticleSystem> skillIndicatorEffectPrefabs; //Ativa a animação de skill pronta para uso --- Hero1 = 0 à 2    Hero2  3 à 5
    [HideInInspector]
    public List<List<GameObject>> tmpIconHeroAtkList = new List<List<GameObject>>();
    public List<GameObject> cloneDamageEffectList = new List<GameObject>();
    public List<Vector3> newPosDamageEffectList = new List<Vector3>();
    public List<GameObject> iconAtkList; // pega a posição dese objecto como referencia para instanciar os icones de indicação de atk de cada herói
    private Vector3 iconHeroAtkPos;
    private Vector3 randomPos;
    [HideInInspector]
    public int nSequence;
    public List<Image> imgSelectedHighLight;
    public List<Image> imgShadowSelectedHighLight;
    public List<Image> imgItemSelectedHighLight;
    public List<Image> imgItemShadowSelectedHighLight;
    private readonly float showWinPanelSeconds = 4f;

    #region TargetSelect Variables
    private int count;
    private int tmpEnemyTgt;
    private float posX;
    private int listCount;
    private int? removeItem;
    private BattleManagementButtons battleManagementButtons;
    #endregion

    #endregion

    #region Options Menu Stuff
    [Header("                                    Options Menu Stuff")]
    public GameObject optionMenu;
    public GameObject btnExitBattle;
    public GameObject btnExitConfirm;
    public GameObject btnExitCancel;
    public GameObject btnCloseOptions;
    public GameObject txtExitBattle;
    #endregion

    #region Win/Lose Panel Stuff
    [Header("                                    Win/Lose Panel Stuff")]
    public GameObject winPanel;
    public GameObject losePanel;
    public ScrollRect scrollRectDrops;
    public List<GameObject> btnDroppedItemList;
    public List<GameObject> txtInfoDroppedItemList;
    public List<GameObject> txtQuantityDroppedItemList;
    public TMP_Text txtPlayerGold;
    public TMP_Text txtGoldReceived;
    private DropItem baseDropItem;
    private int nItems;
    #endregion

    #region Uncategorized Stuff  .-.
    [Header("                                    Uncategorized Stuff .-.")]
    public List<GameObject> toActiveBeforeBattle;
    private MapMainMenu mapMainMenu;
    #endregion

    void Start()
    {
        cloneDamageEffectList = new List<GameObject>();
        tmpIconHeroAtkList.Add(new List<GameObject>());
        tmpIconHeroAtkList.Add(new List<GameObject>());
        tmpIconHeroAtkList.Add(new List<GameObject>());
        randomN = new System.Random();
        enemyTargetSelected = null;
        heroTargetSelected = null;
        utilityTools = FindFirstObjectByType<UtilityTools>();
        heroBars = this.gameObject.AddComponent<HeroBars>();
        enemyBars = FindFirstObjectByType<EnemyBars>();
        heroList = new List<Hero>();
        heroBars.Start();
        enemyBars.Start();
        board = FindFirstObjectByType<Board>();
        mapMainMenu = FindFirstObjectByType<MapMainMenu>();
    }

    private void Update()
    {
        if (cloneDamageEffectList.Count > 0)
        {
            if (cloneDamageEffectList[0] != null && cloneDamageEffectList[0].transform.position != newPosDamageEffectList[0])
            {
                cloneDamageEffectList[0].transform.position = Vector3.Lerp(cloneDamageEffectList[0].gameObject.transform.position, newPosDamageEffectList[0], 0.1f);
            }
        }
    }

    public void LoadInfoInBattleCards()
    {
        #region Variables
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        battleManagementButtons = FindFirstObjectByType<BattleManagementButtons>();
        battleManagementButtons.btnDesActiveSkill.gameObject.SetActive(false);
        heroesManager = FindFirstObjectByType<HeroesManager>();
        levelManager = FindFirstObjectByType<LevelManager>();
        inventory = FindFirstObjectByType<InventoryManager>();
        enemy1 = null;
        enemy2 = null;
        enemy3 = null;
        enemyList = null;
        enemyList = new List<Enemy>();
        heroList = null;
        heroList = new List<Hero>();
        heroBars.fillHPBars = null;
        heroBars.fillHPBars = new List<float>() { 0, 0, 0, 0 };
        enemyList = null;
        enemyList = new List<Enemy>();
        enemyBars.fillHPBars = null;
        enemyBars.fillHPBars = new List<float>() { 0, 0, 0 };

        for (int i = 0; i < toActiveBeforeBattle.Count; i++)
        {
            toActiveBeforeBattle[i].SetActive(true);
            if (i < skillIndicatorEffectPrefabs.Count)
            {
                skillIndicatorEffectPrefabs[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < iconAtkList.Count; i++)
        {
            if (iconAtkList[i] != null)
            {
                for (int x = 0; x < iconAtkList[i].transform.childCount; x++)
                {
                    if (iconAtkList[i].transform.GetChild(x) != null)
                        Destroy(iconAtkList[i].transform.GetChild(x).gameObject);
                }
                iconAtkList[i].SetActive(false);
            }
        }

        tmpIconHeroAtkList.Clear();
        tmpIconHeroAtkList.Add(new List<GameObject>());
        tmpIconHeroAtkList.Add(new List<GameObject>());
        tmpIconHeroAtkList.Add(new List<GameObject>());

        heroBars.Start();
        enemyBars.Start();
        #endregion

        #region Load Hero Info

        for (int i = 0; i < heroesManager.playerTeams.Find(x => x.Id == heroesManager.currenSelectedTeam + 1).teamHeroes.Count; i++)
        {
            heroBase = heroesManager.playerTeams.Find(x => x.Id == heroesManager.currenSelectedTeam + 1).teamHeroes[i];
            heroList.Add(
                new Hero
                {
                    id = heroBase.id,
                    idPlayerHero = heroBase.idPlayerHero,
                    idTeamsList = heroBase.idTeamsList,
                    idSlotTeamList = heroBase.idSlotTeamList,
                    name = heroBase.name,
                    level = heroBase.level,
                    stars = heroBase.stars,
                    spriteStars = heroBase.spriteStars,
                    atk = utilityTools.ReturnStatsIncreasedByTalent(heroBase.atkBase, heroBase.level, heroBase.talentsTreeHero.atkLevel),
                    atkBase = heroBase.atkBase,
                    currentDmg = heroBase.currentDmg,
                    def = (heroBase.defBase * heroBase.level),
                    defBase = heroBase.defBase,
                    life = utilityTools.ReturnStatsIncreasedByTalent(heroBase.lifeBase, heroBase.level, heroBase.talentsTreeHero.hpLevel),
                    lifeBase = heroBase.lifeBase,
                    block = heroBase.block + heroBase.talentsTreeHero.blockLevel,
                    blockBase = heroBase.blockBase,
                    evasion = heroBase.evasionBase + heroBase.talentsTreeHero.evasionLevel,
                    evasionBase = heroBase.evasionBase,
                    threat = heroBase.threat,
                    statusBattle = heroBase.statusBattle,
                    classH = heroBase.classH,
                    exp = heroBase.exp,
                    elementType = heroBase.elementType,
                    elementColor = heroBase.elementColor,
                    powerColor = heroBase.powerColor,
                    dotSkillColor = heroBase.dotSkillColor,
                    maxLevel = heroBase.maxLevel,
                    spriteBattle = heroBase.spriteBattle,
                    spriteCard = heroBase.spriteCard,
                    spriteIconAtk = heroBase.spriteIconAtk,
                    spriteDetailCard = heroBase.spriteDetailCard,
                    spriteHeroImg = heroBase.spriteHeroImg,
                    skillHero = heroBase.skillHero
                }
            );
        }

        var newPsColor = skillIndicatorEffectPrefabs[0].GetComponent<ParticleSystem>().main;
        Color newColor = new Color();

        for (int i = 0; i < heroList.Count; i++)
        {
            if (heroList[i].statusBattle == StatusBattle.ALIVE)
            {
                imgHeroBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
                heroBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
                heroBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
                heroList[i].skillHero.isSkillAvailable = false;
                heroList[i].life = heroList[i].life;
                heroList[i].def = heroList[i].def;
                heroList[i].cloneIconIndice = 0;
                heroList[i].enemyTarget = null;
                txtLifeHeroes[i].text = heroList[i].life.ToString();
                heroBars.fillHPBars[i] = 100f / heroList[i].life;
                heroBars.imgBkgResourceBars[i].sprite = heroBars.spriteBkgResourceBarList[heroList[i].elementType.GetHashCode()];
                heroBars.imgResourceBars[i].sprite = heroBars.spriteResourceBarList[heroList[i].elementType.GetHashCode()];
                heroBars.imgSkillStatus[i].sprite = heroBars.spriteSkillBarList[heroList[i].elementType.GetHashCode()];
                heroBars.imgSkillStatus[i].color = new Color32(55, 55, 55, 255); //No Interactable
                heroBars.imgSkillStatus[i].gameObject.transform.GetChild(0).gameObject.SetActive(false); //Esconde Dot
                imgHeroDotSkillStatus[i].color = heroList[i].dotSkillColor;
                imgHeroDotSkillStatus[i].gameObject.SetActive(false);

                newPsColor = skillIndicatorEffectPrefabs[i * 3 + 0].GetComponent<ParticleSystem>().main;
                newColor = heroList[i].dotSkillColor;
                newColor.a = 0.365f;
                newPsColor.startColor = newColor;

                newPsColor = skillIndicatorEffectPrefabs[i * 3 + 1].GetComponent<ParticleSystem>().main;
                newColor = heroList[i].dotSkillColor;
                newColor.a = 0.365f;
                newPsColor.startColor = newColor;

                newPsColor = skillIndicatorEffectPrefabs[i * 3 + 2].GetComponent<ParticleSystem>().main;
                newColor = heroList[i].dotSkillColor;
                newColor.a = 0.365f;
                newPsColor.startColor = newColor;

                imgSelectedHighLight[i].color = newColor;
                newColor.a = 0.447f;
                imgShadowSelectedHighLight[i].color = newColor;

                #region Battle Info Card

                bkgImageList[i].sprite = heroList[i].spriteDetailCard;
                imgMoldureSelectList[i].color = heroList[i].dotSkillColor;
                newColor = heroList[i].dotSkillColor;
                newColor.a = 0.447f;
                imgShadowMoldureSelectList[i].color = newColor;
                imgIconSkillList[i].sprite = heroList[i].skillHero.icon;
                imgIconMoldureList[i].color = heroList[i].dotSkillColor;
                bkgSkillNameList[i].color = heroList[i].dotSkillColor;
                txtHeroLifeList[i].text = "HP:" + heroList[i].life.ToString();
                txtHeroAtkList[i].text = "ATK:" + heroList[i].atk.ToString();
                txtHeroDefList[i].text = "DEF:" + heroList[i].def.ToString();
                txtResourceList[i].text = "RES:0%";
                txtSkillDesList[i].text = heroList[i].skillHero.detail;
                txtSkillNameList[i].text = heroList[i].skillHero.nameS;
                txtHeroNameList[i].text = heroList[i].name;

                #endregion
            }
            else
            {
                bkgImageList[i].sprite = null;
                txtHeroLifeList[i].text = "";
                txtResourceList[i].text = "";
                txtLifeHeroes[i].text = "";

                if (heroBars.imgHPBars[i] != null)
                {
                    heroBars.imgHPBars[i].gameObject.SetActive(false);
                    heroBars.imgBkgHPBars[i].gameObject.SetActive(false);
                    heroBars.imgResourceBars[i].gameObject.SetActive(false);
                    heroBars.imgBkgResourceBars[i].gameObject.SetActive(false);
                    heroBars.imgSkillStatus[i].gameObject.SetActive(false);
                }
            }

            imgHeroBattleList[i].sprite = heroList[i].spriteBattle;
        }

        #endregion

        #region Load Enemy Info

        enemy1 = levelManager.levelsContainer.currentLevelOpen.levelEnemysList[0];
        enemy1.statusBattle = levelManager.levelsContainer.currentLevelOpen.enemyLevelStatusBattleBase[0];
        enemy2 = levelManager.levelsContainer.currentLevelOpen.levelEnemysList[1];
        enemy2.statusBattle = levelManager.levelsContainer.currentLevelOpen.enemyLevelStatusBattleBase[1];
        enemy3 = levelManager.levelsContainer.currentLevelOpen.levelEnemysList[2];
        enemy3.statusBattle = levelManager.levelsContainer.currentLevelOpen.enemyLevelStatusBattleBase[2];

        enemyList.Add(enemy1);
        enemyList.Add(enemy2);
        enemyList.Add(enemy3);

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (enemyBars.imgHPBars[i] != null)
                {
                    enemyBars.imgHPBars[i].gameObject.SetActive(true);
                    enemyBars.imgBkgHPBars[i].gameObject.SetActive(true);
                    imgEnemyBattleList[i].gameObject.SetActive(true);
                }

                imgEnemyBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
                enemyBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
                enemyBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
                txtLifeEnemys[i].text = enemyList[i].lifeBase.ToString();
                enemyList[i].life = enemyList[i].lifeBase;
                enemyBars.fillHPBars[i] = 100f / enemyList[i].lifeBase;
                imgEnemyBattleList[i].sprite = enemyList[i].spriteBattle;
            }
            else
            {
                if (enemyBars.imgHPBars[i] != null)
                {
                    enemyBars.imgHPBars[i].gameObject.SetActive(false);
                    enemyBars.imgBkgHPBars[i].gameObject.SetActive(false);
                    txtLifeEnemys[i].text = "";
                    imgEnemyBattleList[i].gameObject.SetActive(false);
                }
            }
        }

        #endregion

        #region Load BattleItems Info        
        for (int i = 0; i < btnBattleItemsList.Count; i++)
        {
            if (inventory.teamItems[heroesManager.currenSelectedTeam] != null)
            {
                btnBattleItemsList[i].interactable = true;
                btnBattleItemsList[i].GetComponent<Image>().sprite = inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[i].spriteNormalSize;

                if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[i].quantityInUse > 0)
                {
                    itemQuantity[i].text = "x" + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[i].quantityInUse.ToString();
                    txtItemInfo[i].text = inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[i].infoItem.ToString();
                    newColor = inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[i].colorOnUse;

                    imgItemSelectedHighLight[i].color = newColor;
                    newColor.a = 0.447f;
                    imgItemShadowSelectedHighLight[i].color = newColor;
                }
                else
                {
                    itemQuantity[i].text = "";
                    txtItemInfo[i].text = "";
                    if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[i].itemType != ItemType.Empty)
                    {
                        btnBattleItemsList[i].interactable = false;
                        itemQuantity[i].text = "x0";
                        txtItemInfo[i].text = inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[i].infoItem.ToString();
                    }
                }

            }
        }
        #endregion

        #region Load Dropped Items Info
        for (int i = 0; i < btnDroppedItemList.Count; i++)
        {
            if (i < levelManager.levelsContainer.currentLevelOpen.levelDropItems.Count)
            {
                btnDroppedItemList[i].gameObject.SetActive(true);
                btnDroppedItemList[i].GetComponent<Image>().sprite = levelManager.levelsContainer.currentLevelOpen.levelDropItems[i].spriteNormalSize;
                txtInfoDroppedItemList[i].GetComponent<Text>().text = levelManager.levelsContainer.currentLevelOpen.levelDropItems[i].name;
                txtQuantityDroppedItemList[i].GetComponent<Text>().text = "x" + levelManager.levelsContainer.currentLevelOpen.levelDropItems[i].quantity.ToString();
            }
            else
            {
                txtInfoDroppedItemList[i].GetComponent<Text>().text = "";
                txtQuantityDroppedItemList[i].GetComponent<Text>().text = "";
                btnDroppedItemList[i].gameObject.SetActive(false);
            }
        }
        #endregion
    }

    public void ExitSelectedMode(bool SelectedMode)
    {
        if (SelectedMode == false)
        {
            this.SetDefaultColorsSelectedMode();
            enemyTargetSelected = null;
            heroTargetSelected = null;
            nItemSelected = null;
            board.currentState = GameState.move;
        }
    }

    public void SelectHeroTarget(int nHero)
    {
        heroTargetSelected = nHero;

        for (int i = 0; i < heroList.Count; i++)
        {
            if (heroList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (imgHeroBattleList[i] != null && i != nHero)
                    imgHeroBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (heroBars.imgHPBars[i] != null && i != nHero)
                    heroBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (heroBars.imgBkgHPBars[i] != null && i != nHero)
                    heroBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (imgHeroBattleList[i] != null && i == nHero)
                    imgHeroBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (heroBars.imgHPBars[i] != null && i == nHero)
                    heroBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (heroBars.imgBkgHPBars[i] != null && i == nHero)
                    heroBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
            }
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (imgEnemyBattleList[i] != null)
                    imgEnemyBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (enemyBars.imgHPBars[i] != null)
                    enemyBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (enemyBars.imgBkgHPBars[i] != null)
                    enemyBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

            }
        }

        StartCoroutine(SetHeroTarget());
    }

    public void SelectEnemyTarget(int nEnemy)
    {
        enemyTargetSelected = nEnemy;

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (imgEnemyBattleList[i] != null && i != nEnemy)
                    imgEnemyBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (enemyBars.imgHPBars[i] != null && i != nEnemy)
                    enemyBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (enemyBars.imgBkgHPBars[i] != null && i != nEnemy)
                    enemyBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (imgEnemyBattleList[i] != null && i == nEnemy)
                    imgEnemyBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (enemyBars.imgHPBars[i] != null && i == nEnemy)
                    enemyBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (enemyBars.imgBkgHPBars[i] != null && i == nEnemy)
                    enemyBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
            }
        }

        for (int i = 0; i < heroList.Count; i++)
        {
            if (heroList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (imgHeroBattleList[i] != null)
                    imgHeroBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (heroBars.imgHPBars[i] != null)
                    heroBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);

                if (heroBars.imgBkgHPBars[i] != null)
                    heroBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);
            }
        }

        StartCoroutine(SetHeroTarget());
    }

    public IEnumerator SetHeroTarget()
    {
        if (heroTargetSelected.HasValue && enemyTargetSelected.HasValue)
        {
            if (heroList[heroTargetSelected.GetValueOrDefault()].enemyTarget.HasValue == false)
            {
                #region Add Icon-Atk Indicator

                heroList[heroTargetSelected.GetValueOrDefault()].enemyTarget = enemyTargetSelected.GetValueOrDefault();

                iconAtkList[enemyTargetSelected.GetValueOrDefault()].SetActive(true);
                posX = iconAtkList[enemyTargetSelected.GetValueOrDefault()].transform.position.x;
                iconHeroAtkPos = iconAtkList[enemyTargetSelected.GetValueOrDefault()].transform.position;
                iconHeroAtkPos.y -= .9f;

                if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 0)
                    iconHeroAtkPos.x = posX;

                foreach (GameObject iconHeroObj in tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()])
                {
                    if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 1)
                    {
                        iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                        iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
                    }

                    if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 2)
                    {
                        iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                        iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
                    }

                    if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 3)
                    {
                        iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                        iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
                    }
                }

                var cloneIconHeroAtk1 = Instantiate(iconHeroAtkPrefab, iconHeroAtkPos, Quaternion.identity, iconAtkList[enemyTargetSelected.GetValueOrDefault()].transform);
                cloneIconHeroAtk1.GetComponent<Image>().sprite = heroList[heroTargetSelected.GetValueOrDefault()].spriteIconAtk;

                heroList[heroTargetSelected.GetValueOrDefault()].cloneIconIndice = tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count();
                tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Add(cloneIconHeroAtk1);

                enemyTargetSelected = null;
                heroTargetSelected = null;

                yield return new WaitForSeconds(.0f);

                this.SetDefaultColorsSelectedMode();
                #endregion 
            }
            else
            {
                #region Remove->Add   Icon-Atk Indicator

                tmpEnemyTgt = heroList[heroTargetSelected.GetValueOrDefault()].enemyTarget.GetValueOrDefault();

                if (tmpEnemyTgt != enemyTargetSelected.GetValueOrDefault())
                {
                    iconAtkList[enemyTargetSelected.GetValueOrDefault()].SetActive(true);

                    posX = iconAtkList[enemyTargetSelected.GetValueOrDefault()].transform.position.x;
                    iconHeroAtkPos = iconAtkList[enemyTargetSelected.GetValueOrDefault()].transform.position;
                    iconHeroAtkPos.y -= .9f;

                    foreach (List<GameObject> cloneObjList in tmpIconHeroAtkList)
                    {
                        listCount = cloneObjList.Count();
                        removeItem = null;

                        if (count == tmpEnemyTgt)
                        {
                            int cloneIndice = heroList[heroTargetSelected.GetValueOrDefault()].cloneIconIndice;

                            for (int i = 0; i < listCount; i++)
                            {
                                if (listCount == 1)
                                {
                                    Destroy(cloneObjList[i].gameObject);
                                    cloneObjList.RemoveAt(i);
                                    iconAtkList[tmpEnemyTgt].SetActive(false);
                                }

                                if (listCount == 2)
                                {
                                    if (cloneIndice == i)
                                    {
                                        Destroy(cloneObjList[i].gameObject);
                                        removeItem = i;
                                    }
                                    else
                                    {
                                        if (cloneIndice == 0)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                           && x.cloneIconIndice == 1).FirstOrDefault().cloneIconIndice = 0;
                                        }

                                        if (cloneIndice == 1)
                                            cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                                    }
                                }

                                if (listCount == 3)
                                {
                                    if (cloneIndice == i)
                                    {
                                        Destroy(cloneObjList[i].gameObject);
                                        removeItem = i;
                                    }
                                    else
                                    {
                                        if (cloneIndice == 0 && i == 1)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 1).FirstOrDefault().cloneIconIndice = 0;
                                        }

                                        if (cloneIndice == 0 && i == 2)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                                        }

                                        if (cloneIndice == 1 && i == 0)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                                        }

                                        if (cloneIndice == 1 && i == 2)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                                        }

                                        if (cloneIndice == 2)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                                        }
                                    }
                                }

                                if (listCount == 4)
                                {
                                    if (cloneIndice == i)
                                    {
                                        Destroy(cloneObjList[i].gameObject);
                                        removeItem = i;
                                    }
                                    else
                                    {
                                        if (cloneIndice == 0 && i == 1)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 1).FirstOrDefault().cloneIconIndice = 0;
                                        }

                                        if (cloneIndice == 0 && i == 2)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                                        }

                                        if (cloneIndice == 0 && i == 3)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 3).FirstOrDefault().cloneIconIndice = 2;
                                        }


                                        if (cloneIndice == 1 && i == 0)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                                        }

                                        if (cloneIndice == 1 && i == 2)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                                        }

                                        if (cloneIndice == 1 && i == 3)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                            && x.cloneIconIndice == 3).FirstOrDefault().cloneIconIndice = 2;
                                        }


                                        if (cloneIndice == 2 && i == 0)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                                        }

                                        if (cloneIndice == 2 && i == 1)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                                        }

                                        if (cloneIndice == 2 && i == 3)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                            heroList.Where(x => x.enemyTarget == tmpEnemyTgt
                                                             && x.cloneIconIndice == 3).FirstOrDefault().cloneIconIndice = 2;
                                        }



                                        if (cloneIndice == 3)
                                        {
                                            cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                                        }
                                    }
                                }
                            }
                            if (removeItem.HasValue)
                                cloneObjList.RemoveAt(removeItem.Value);

                        }
                        count++;
                    }

                    count = 0;

                    heroList[heroTargetSelected.GetValueOrDefault()].enemyTarget = enemyTargetSelected.GetValueOrDefault();

                    if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 0)
                        iconHeroAtkPos.x = posX;

                    foreach (GameObject iconHeroObj in tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()])
                    {
                        if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 1)
                        {
                            iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                            iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
                        }

                        if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 2)
                        {
                            iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                            iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
                        }

                        if (tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count == 3)
                        {
                            iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                            iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
                        }
                    }

                    //Cria Prefab Icon Atk 
                    var cloneIconHeroAtk1 = Instantiate(iconHeroAtkPrefab, iconHeroAtkPos, Quaternion.identity, iconAtkList[enemyTargetSelected.GetValueOrDefault()].transform);
                    cloneIconHeroAtk1.GetComponent<Image>().sprite = heroList[heroTargetSelected.GetValueOrDefault()].spriteIconAtk;

                    heroList[heroTargetSelected.GetValueOrDefault()].cloneIconIndice = tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Count();
                    tmpIconHeroAtkList[enemyTargetSelected.GetValueOrDefault()].Add(cloneIconHeroAtk1);

                    enemyTargetSelected = null;
                    heroTargetSelected = null;

                    this.SetDefaultColorsSelectedMode();
                }
                #endregion
            }
            yield return new WaitForSeconds(0f);
        }
    }

    public IEnumerator SetHeroTarget(int tmpIdHero, int tmpNewTarget, int? tmpOldTarget)
    {
        yield return new WaitForSeconds(0f);

        #region Remove->Add   Icon-Atk Indicator  

        int localCount = 0;
        int localListCount = 0;

        iconAtkList[tmpNewTarget].SetActive(true);
        posX = iconAtkList[tmpNewTarget].transform.position.x;
        iconHeroAtkPos = iconAtkList[tmpNewTarget].transform.position;
        iconHeroAtkPos.y -= .9f;

        foreach (List<GameObject> cloneObjList in tmpIconHeroAtkList)
        {
            localListCount = cloneObjList.Count();
            int? localRemoveItem = null;

            if (localCount == tmpOldTarget)
            {
                int cloneIndice = heroList[tmpIdHero].cloneIconIndice;

                for (int i = 0; i < localListCount; i++)
                {
                    if (localListCount == 1)
                    {
                        Destroy(cloneObjList[i].gameObject);
                        cloneObjList.RemoveAt(i);
                        iconAtkList[tmpOldTarget.Value].SetActive(false);
                    }

                    if (localListCount == 2)
                    {
                        if (cloneIndice == i)
                        {
                            Destroy(cloneObjList[i].gameObject);
                            localRemoveItem = i;
                        }
                        else
                        {
                            if (cloneIndice == 0)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                               && x.cloneIconIndice == 1).FirstOrDefault().cloneIconIndice = 0;
                            }

                            if (cloneIndice == 1)
                                cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                        }
                    }

                    if (localListCount == 3)
                    {
                        if (cloneIndice == i)
                        {
                            Destroy(cloneObjList[i].gameObject);
                            localRemoveItem = i;
                        }
                        else
                        {
                            if (cloneIndice == 0 && i == 1)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 1).FirstOrDefault().cloneIconIndice = 0;
                            }

                            if (cloneIndice == 0 && i == 2)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                            }

                            if (cloneIndice == 1 && i == 0)
                            {
                                cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                            }

                            if (cloneIndice == 1 && i == 2)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                            }

                            if (cloneIndice == 2)
                            {
                                cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                            }
                        }
                    }

                    if (localListCount == 4)
                    {
                        if (cloneIndice == i)
                        {
                            Destroy(cloneObjList[i].gameObject);
                            localRemoveItem = i;
                        }
                        else
                        {
                            if (cloneIndice == 0 && i == 1)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 1).FirstOrDefault().cloneIconIndice = 0;
                            }

                            if (cloneIndice == 0 && i == 2)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                            }

                            if (cloneIndice == 0 && i == 3)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 3).FirstOrDefault().cloneIconIndice = 2;
                            }


                            if (cloneIndice == 1 && i == 0)
                            {
                                cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                            }

                            if (cloneIndice == 1 && i == 2)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 2).FirstOrDefault().cloneIconIndice = 1;
                            }

                            if (cloneIndice == 1 && i == 3)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                && x.cloneIconIndice == 3).FirstOrDefault().cloneIconIndice = 2;
                            }


                            if (cloneIndice == 2 && i == 0)
                            {
                                cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                            }

                            if (cloneIndice == 2 && i == 1)
                            {
                                cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                            }

                            if (cloneIndice == 2 && i == 3)
                            {
                                cloneObjList[i].GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                                heroList.Where(x => x.enemyTarget == tmpOldTarget
                                                 && x.cloneIconIndice == 3).FirstOrDefault().cloneIconIndice = 2;
                            }

                            if (cloneIndice == 3)
                            {
                                cloneObjList[i].GetComponent<Transform>().position += new Vector3(.35f, 0, 0);
                            }
                        }
                    }
                }
                if (localRemoveItem.HasValue)
                    cloneObjList.RemoveAt(localRemoveItem.Value);

            }
            localCount++;
        }

        if (tmpIconHeroAtkList[tmpNewTarget].Count == 0)
            iconHeroAtkPos.x = posX;

        foreach (GameObject iconHeroObj in tmpIconHeroAtkList[tmpNewTarget])
        {
            if (tmpIconHeroAtkList[tmpNewTarget].Count == 1)
            {
                iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
            }

            if (tmpIconHeroAtkList[tmpNewTarget].Count == 2)
            {
                iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
            }

            if (tmpIconHeroAtkList[tmpNewTarget].Count == 3)
            {
                iconHeroObj.GetComponent<Transform>().position -= new Vector3(.35f, 0, 0);
                iconHeroAtkPos.x = iconHeroObj.transform.position.x + .7f;
            }
        }

        //Cria Prefab Icon Atk  
        var cloneIconHeroAtk1 = Instantiate(iconHeroAtkPrefab, iconHeroAtkPos, Quaternion.identity, iconAtkList[tmpNewTarget].transform);
        cloneIconHeroAtk1.GetComponent<Image>().sprite = heroList[tmpIdHero].spriteIconAtk;

        heroList[tmpIdHero].cloneIconIndice = tmpIconHeroAtkList[tmpNewTarget].Count();
        heroList[tmpIdHero].enemyTarget = tmpNewTarget;
        tmpIconHeroAtkList[tmpNewTarget].Add(cloneIconHeroAtk1);

        #endregion
    }

    public void SelectEnemySkillTargetToHarm(int nEnemy)
    {
        if (heroList[heroTargetSelected.Value].skillHero.isSkillAvailable == true)
        {
            heroList[heroTargetSelected.Value].skillHero.isSkillAvailable = false;
            imgSelectedHighLight[heroTargetSelected.Value].gameObject.SetActive(false);
            imgShadowSelectedHighLight[heroTargetSelected.Value].gameObject.SetActive(false);
            enemyTargetSelected = nEnemy;

            StartCoroutine(SetHeroSkillTarget());
        }
    }

    public void SelectHeroSkillTargetToHelp(int nHero)
    {
        if (heroList[heroTargetSelected.Value].skillHero.isSkillAvailable == true)
        {
            heroList[heroTargetSelected.Value].skillHero.isSkillAvailable = false;
            imgSelectedHighLight[heroTargetSelected.Value].gameObject.SetActive(false);
            imgShadowSelectedHighLight[heroTargetSelected.Value].gameObject.SetActive(false);
            heroTargetSelectedToHelp = nHero;

            StartCoroutine(SetHeroSkillTarget());
        }
    }

    public void SelectHeroSkillTarget(int nHero)
    {
        if ((board.currentState == GameState.move || board.currentState == GameState.skillTargetSelect)
            && heroList[nHero].statusBattle == StatusBattle.ALIVE
            && heroList[nHero].skillHero.isSkillAvailable == true)
        {
            board.currentState = GameState.skillTargetSelect;
            battleManagementButtons.btnActiveSelecTargetMode.gameObject.SetActive(false);
            battleManagementButtons.btnDesActiveSkill.gameObject.SetActive(true);
            imgSelectedHighLight[nHero].gameObject.SetActive(true);
            imgShadowSelectedHighLight[nHero].gameObject.SetActive(true);

            heroTargetSelected = nHero;

            for (int i = 0; i < battleManagementButtons.btnHeroTargetList.Count; i++)
            {
                if (heroList[i].statusBattle == StatusBattle.ALIVE
                    && heroList[nHero].skillHero.skillType == SkillType.HELP
                    && battleManagementButtons.btnHeroTargetList[i] != null)
                {
                    battleManagementButtons.btnHeroTargetList[i].gameObject.SetActive(true);
                    battleManagementButtons.btnHeroTargetSkillListToHelp[i].gameObject.SetActive(true);
                }

                if (i != nHero)
                {
                    imgSelectedHighLight[i].gameObject.SetActive(false);
                    imgShadowSelectedHighLight[i].gameObject.SetActive(false);

                    if (heroList[nHero].skillHero.skillType == SkillType.HELP)
                        battleManagementButtons.btnHeroActiveSkillList[i].gameObject.SetActive(false);
                }

                if (heroList[i].statusBattle == StatusBattle.DEAD && heroList[i].isRessable == true)
                {
                    battleManagementButtons.btnHeroTargetList[i].gameObject.SetActive(true);
                    battleManagementButtons.btnHeroTargetSkillListToHelp[i].gameObject.SetActive(true);
                }

                if (enemyList.Count > i && enemyList[i].statusBattle == StatusBattle.ALIVE
                    && heroList[nHero].skillHero.skillType == SkillType.HARM
                    && battleManagementButtons.btnEnemyTargetList[i] != null)
                {
                    battleManagementButtons.btnEnemyTargeSkillListToHarm[i].gameObject.SetActive(true);
                    battleManagementButtons.btnEnemyTargetList[i].gameObject.SetActive(true);
                }

                if (enemyList.Count > i
                    && heroList[nHero].skillHero.skillType == SkillType.HELP
                    && battleManagementButtons.btnEnemyTargetList[i] != null)
                {
                    battleManagementButtons.btnEnemyTargeSkillListToHarm[i].gameObject.SetActive(false);
                    battleManagementButtons.btnEnemyTargetList[i].gameObject.SetActive(false);
                }
            }

            StartCoroutine(SetHeroSkillTarget());
        }
    }

    public IEnumerator SetHeroSkillTarget()
    {
        yield return new WaitForSeconds(0f);

        if (heroTargetSelected.HasValue && enemyTargetSelected.HasValue)
        {
            #region Direct Damage Skills (Target Selecting)
            //Conjura habilidades em um alvo escolhido pelo jogador 

            if (enemyList[enemyTargetSelected.Value].statusBattle == StatusBattle.ALIVE)
            {
                battleManagementButtons.btnHeroActiveSkillList[heroTargetSelected.Value].gameObject.SetActive(false);

                StartCoroutine(ShowFloatTexting
                (
                    imgHeroBattleList[heroTargetSelected.Value].gameObject
                    , imgEnemyBattleList[enemyTargetSelected.Value].gameObject
                    , heroList[heroTargetSelected.Value].skillHero.applyDamage.ToString()
                    , UIColorTemplate.DAMAGE
                    , heroList[heroTargetSelected.Value].elementColor)
                );

                StartCoroutine(enemyBars.DecreaseEnemyHP(heroList[heroTargetSelected.Value].skillHero.applyDamage, enemyTargetSelected.Value));
                heroBars.DecreaseHeroResource(heroTargetSelected.Value);

                randomDamage = (enemyList[enemyTargetSelected.Value].life - heroList[heroTargetSelected.Value].skillHero.applyDamage) > 0 ? (enemyList[enemyTargetSelected.Value].life - heroList[heroTargetSelected.Value].skillHero.applyDamage) : 0;

                if (randomDamage == 0)
                {
                    enemyList[enemyTargetSelected.Value].statusBattle = StatusBattle.DEAD;
                    battleManagementButtons.btnEnemyTargeSkillListToHarm[enemyTargetSelected.Value].gameObject.SetActive(false);
                    battleManagementButtons.btnEnemyTargetList[enemyTargetSelected.Value].gameObject.SetActive(false);
                }

                StartCoroutine(UpdateUIEnemyInfo(enemyTargetSelected.Value, randomDamage));
                enemyList[enemyTargetSelected.Value].life = randomDamage;

                //HeroResource
                txtResourceList[heroTargetSelected.Value].text = "RES:0%";

                enemyTargetSelected = null;

                if (enemyList.Where(x => x.statusBattle == StatusBattle.ALIVE).Count() == 0)
                {
                    board.currentState = GameState.win;
                    StartCoroutine(this.ShowWinPanel(showWinPanelSeconds));
                }
            }

            #endregion            

            //battleManagementButtons.btnActiveSelecTargetMode.gameObject.SetActive(false);
            //battleManagementButtons.btnDesActiveSkill.gameObject.SetActive(false);
            battleManagementButtons.ExitSelectedMode(false);
        }

        if (heroTargetSelected.HasValue && heroTargetSelectedToHelp.HasValue)
        {
            #region Direct Healing Skills (Target Selecting)
            //Conjura habilidades em um alvo escolhido pelo jogador 
            battleManagementButtons.btnHeroActiveSkillList[heroTargetSelected.Value].gameObject.SetActive(false);

            StartCoroutine(ShowFloatTexting
            (
                imgHeroBattleList[heroTargetSelected.Value].gameObject
                , imgHeroBattleList[heroTargetSelectedToHelp.Value].gameObject
                , heroList[heroTargetSelected.Value].skillHero.fillHeal.ToString()
                , UIColorTemplate.HEAL
                , heroList[heroTargetSelected.Value].elementColor)
            );

            StartCoroutine(heroBars.IncreaseHeroHP(heroList[heroTargetSelected.Value].skillHero.fillHeal, heroTargetSelectedToHelp.Value));
            heroBars.DecreaseHeroResource(heroTargetSelected.Value);

            //heal hero target
            randomDamage = (heroList[heroTargetSelectedToHelp.Value].life + heroList[heroTargetSelected.Value].skillHero.fillHeal) < (heroList[heroTargetSelectedToHelp.Value].lifeBase * heroList[heroTargetSelectedToHelp.Value].level)
                           ? (heroList[heroTargetSelectedToHelp.Value].life + heroList[heroTargetSelected.Value].skillHero.fillHeal)
                           : (heroList[heroTargetSelectedToHelp.Value].lifeBase * heroList[heroTargetSelectedToHelp.Value].level);

            //HeroResource
            txtResourceList[heroTargetSelected.Value].text = "RES:0%";

            StartCoroutine(UpdateUIHeroInfo(heroTargetSelectedToHelp.Value, randomDamage));
            heroList[heroTargetSelectedToHelp.Value].life = randomDamage;
            txtHeroLifeList[heroTargetSelectedToHelp.Value].text = "HP:" + randomDamage.ToString();
            heroTargetSelectedToHelp = null;
            battleManagementButtons.ExitSelectedMode(false);

            #endregion
        }
    }

    public void SelectItemTarget(int nItem)
    {
        if ((board.currentState == GameState.move || board.currentState == GameState.itemTargetSelect)
            && inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItem].itemType != ItemType.Empty)
        {
            nItemSelected = nItem;
            board.currentState = GameState.itemTargetSelect;
            battleManagementButtons.btnActiveSelecTargetMode.gameObject.SetActive(false);
            battleManagementButtons.btnDesActiveSkill.gameObject.SetActive(true);

            imgItemSelectedHighLight[nItem].gameObject.SetActive(true);
            imgItemShadowSelectedHighLight[nItem].gameObject.SetActive(true);

            for (int i = 0; i < battleManagementButtons.btnHeroTargetList.Count; i++)
            {
                #region Item Type HELP

                if (heroList[i].statusBattle == StatusBattle.ALIVE &&
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItem].itemEffectType == ItemEffectType.HELP &&
                    battleManagementButtons.btnHeroTargetList[i] != null)
                {
                    battleManagementButtons.btnHeroTargetList[i].gameObject.SetActive(true);
                    battleManagementButtons.btnItemTargetListToHelp[i].gameObject.SetActive(true);
                }

                if (i != nItem)
                {
                    imgItemSelectedHighLight[i].gameObject.SetActive(false);
                    imgItemShadowSelectedHighLight[i].gameObject.SetActive(false);
                    imgItemShadowSelectedHighLight[i].gameObject.SetActive(false);
                }

                if (enemyList.Count > i &&
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItem].itemEffectType == ItemEffectType.HELP &&
                    battleManagementButtons.btnEnemyTargetList[i] != null)
                {
                    battleManagementButtons.btnEnemyTargetList[i].gameObject.SetActive(false);
                    battleManagementButtons.btnItemTargeListToHarm[i].gameObject.SetActive(false);
                }

                #endregion

                #region Item Type HARM

                if (enemyList.Count > i && enemyList[i].statusBattle == StatusBattle.ALIVE &&
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItem].itemEffectType == ItemEffectType.HARM &&
                    battleManagementButtons.btnEnemyTargetList[i] != null)
                {
                    battleManagementButtons.btnItemTargeListToHarm[i].gameObject.SetActive(true);
                    battleManagementButtons.btnEnemyTargetList[i].gameObject.SetActive(true);
                }

                if (heroList[i].statusBattle == StatusBattle.ALIVE &&
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItem].itemEffectType == ItemEffectType.HARM &&
                    battleManagementButtons.btnHeroTargetList[i] != null)
                {
                    battleManagementButtons.btnHeroTargetList[i].gameObject.SetActive(false);
                    battleManagementButtons.btnItemTargetListToHelp[i].gameObject.SetActive(false);
                }

                #endregion
            }
        }
    }

    public void SelectItemTargetToHelp(int nHero)
    {
        if ((board.currentState == GameState.move || board.currentState == GameState.itemTargetSelect)
            && inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].itemType != ItemType.Empty)
        {
            heroTargetSelectedToHelp = nHero;
            StartCoroutine(SetItemTarget());
        }
    }

    public void SelectItemTargetToHarm(int nEnemy)
    {
        if ((board.currentState == GameState.move || board.currentState == GameState.itemTargetSelect)
            && inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].itemType != ItemType.Empty)
        {
            enemyTargetSelected = nEnemy;
            StartCoroutine(SetItemTarget());
        }
    }

    public IEnumerator SetItemTarget()
    {
        yield return new WaitForSeconds(0f);

        if (nItemSelected.HasValue && enemyTargetSelected.HasValue)
        {
            inventory = FindFirstObjectByType<InventoryManager>();

            #region Instant Damage Items (Target Selecting)
            //Usa o item em um alvo escolhido pelo jogador 

            if (enemyList[enemyTargetSelected.Value].statusBattle == StatusBattle.ALIVE &&
                inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse > 0)
            {
                StartCoroutine(ShowFloatTexting
                (
                    btnBattleItemsList[nItemSelected.Value].gameObject
                    , imgEnemyBattleList[enemyTargetSelected.Value].gameObject
                    , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].decreaseLife.ToString()
                    , UIColorTemplate.DAMAGE
                    , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].colorOnUse)
                );

                StartCoroutine(enemyBars.DecreaseEnemyHP(inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].decreaseLife, enemyTargetSelected.Value));

                inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse -= 1;
                inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity -= 1;

                inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse;
                inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantity = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity;

                itemQuantity[nItemSelected.Value].text = "x" + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse.ToString();

                if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse == 0)
                    btnBattleItemsList[nItemSelected.Value].interactable = false;

                //Damage Target
                randomDamage = (enemyList[enemyTargetSelected.Value].life - inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].decreaseLife) > 0
                               ? (enemyList[enemyTargetSelected.Value].life - inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].decreaseLife)
                               : 0;

                if (randomDamage == 0)
                {
                    enemyList[enemyTargetSelected.Value].statusBattle = StatusBattle.DEAD;
                    battleManagementButtons.btnEnemyTargeSkillListToHarm[enemyTargetSelected.Value].gameObject.SetActive(false);
                    battleManagementButtons.btnItemTargeListToHarm[enemyTargetSelected.Value].gameObject.SetActive(false);
                }

                StartCoroutine(UpdateUIEnemyInfo(enemyTargetSelected.Value, randomDamage));
                enemyList[enemyTargetSelected.Value].life = randomDamage;
                enemyTargetSelected = null;
                battleManagementButtons.ExitSelectedMode(false);

                if (enemyList.FirstOrDefault(x => x.statusBattle == StatusBattle.ALIVE) == null)
                {
                    board.currentState = GameState.win;
                    StartCoroutine(this.ShowWinPanel(showWinPanelSeconds));
                }
            }

            enemyTargetSelected = null;

            #endregion
        }

        if (nItemSelected.HasValue && heroTargetSelectedToHelp.HasValue)
        {
            if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse > 0)
            {
                inventory = FindFirstObjectByType<InventoryManager>();

                #region Instant Heal Item  (Target Selecting)

                if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].itemType == ItemType.LifePotion &&
                    heroBars.imgHPBars[heroTargetSelectedToHelp.Value].fillAmount < 1)
                {
                    //Usa item em um alvo escolhido pelo jogador 
                    StartCoroutine(ShowFloatTexting
                    (
                        btnBattleItemsList[nItemSelected.Value].gameObject
                        , imgHeroBattleList[heroTargetSelectedToHelp.Value].gameObject
                        , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].restoreLife.ToString()
                        , UIColorTemplate.HEAL
                        , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].colorOnUse)
                    );

                    StartCoroutine(heroBars.IncreaseHeroHP(inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].restoreLife, heroTargetSelectedToHelp.Value));

                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse -= 1;
                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity -= 1;

                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse;
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantity = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity;

                    itemQuantity[nItemSelected.Value].text = "x" + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse.ToString();

                    if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse == 0)
                        btnBattleItemsList[nItemSelected.Value].interactable = false;

                    //heal hero target
                    randomDamage = (heroList[heroTargetSelectedToHelp.Value].life + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].restoreLife) < heroList[heroTargetSelectedToHelp.Value].lifeBase
                                   ? (heroList[heroTargetSelectedToHelp.Value].life + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].restoreLife)
                                   : heroList[heroTargetSelectedToHelp.Value].lifeBase;

                    StartCoroutine(UpdateUIHeroInfo(heroTargetSelectedToHelp.Value, randomDamage));
                    heroList[heroTargetSelectedToHelp.Value].life = randomDamage;
                    txtHeroLifeList[heroTargetSelectedToHelp.Value].text = "HP:" + randomDamage.ToString();

                    heroTargetSelectedToHelp = null;

                    battleManagementButtons.ExitSelectedMode(false);
                }

                #endregion

                #region Instant Fill Resource  (Target Selecting)

                if (nItemSelected.HasValue && inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].itemType == ItemType.ResourcePotion &&
                    heroBars.imgResourceBars[heroTargetSelectedToHelp.Value].fillAmount < 1)
                {
                    //Usa item em um alvo escolhido pelo jogador 
                    StartCoroutine(ShowFloatTexting
                    (
                        btnBattleItemsList[nItemSelected.Value].gameObject
                        , imgHeroBattleList[heroTargetSelectedToHelp.Value].gameObject
                        , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].restoreResource.ToString()
                        , UIColorTemplate.RESOURCE
                        , heroList[heroTargetSelectedToHelp.Value].dotSkillColor)
                    );

                    heroBars.IncreaseHeroResource(inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].restoreResource, heroTargetSelectedToHelp.Value);

                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse -= 1;
                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity -= 1;

                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse;
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantity = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity;

                    itemQuantity[nItemSelected.Value].text = "x" + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse.ToString();

                    if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse == 0)
                        btnBattleItemsList[nItemSelected.Value].interactable = false;


                    heroTargetSelectedToHelp = null;

                    battleManagementButtons.ExitSelectedMode(false);
                }

                #endregion

                #region Buff Items

                if (nItemSelected.HasValue && inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].itemType == ItemType.DefenseRune)
                {
                    #region Defense Rune;                    
                    //Usa item em um alvo escolhido pelo jogador 
                    for (int i = 0; i < heroList.Count; i++)
                    {
                        if (heroList[i].statusBattle == StatusBattle.ALIVE)
                        {
                            StartCoroutine(ShowFloatTexting
                            (
                                btnBattleItemsList[nItemSelected.Value].gameObject
                                , imgHeroBattleList[i].gameObject
                                , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].textOnUse.ToString()
                                , UIColorTemplate.RUNE
                                , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].colorOnUse)
                            );

                            calcBuff = (heroList[i].def * inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].reduceDamage) / 100;

                            if (calcBuff < 1)
                                calcBuff = 1;

                            heroList[i].def += calcBuff;
                            txtHeroDefList[i].text = "DEF:" + (heroList[i].level * heroList[i].defBase) + "(+" + (heroList[i].def - (heroList[i].level * heroList[i].defBase)) + ")";
                        }
                    }

                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse -= 1;
                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity -= 1;

                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse;
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantity = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity;

                    itemQuantity[nItemSelected.Value].text = "x" + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse.ToString();

                    if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse == 0)
                        btnBattleItemsList[nItemSelected.Value].interactable = false;

                    heroTargetSelectedToHelp = null;

                    battleManagementButtons.ExitSelectedMode(false);

                    #endregion
                }
                else if (nItemSelected.HasValue && inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].itemType == ItemType.DamageRune)
                {
                    #region Damage Rune;     
                    //Usa item em um alvo escolhido pelo jogador 

                    for (int i = 0; i < heroList.Count; i++)
                    {
                        if (heroList[i].statusBattle == StatusBattle.ALIVE)
                        {
                            StartCoroutine(ShowFloatTexting
                            (
                                btnBattleItemsList[nItemSelected.Value].gameObject
                                , imgHeroBattleList[i].gameObject
                                , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].textOnUse.ToString()
                                , UIColorTemplate.RUNE
                                , inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].colorOnUse)
                            );

                            calcBuff = ((heroList[i].level * heroList[i].atkBase) * inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].increaseDamage) / 100;

                            if (calcBuff < 1)
                                calcBuff = 1;

                            heroList[i].atk += calcBuff;
                            txtHeroAtkList[i].text = "ATK:" + (heroList[i].level * heroList[i].atkBase) + "(+" + (heroList[i].atk - (heroList[i].level * heroList[i].atkBase)) + ")";
                        }
                    }

                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse -= 1;
                    inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity -= 1;

                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantityInUse;
                    inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantity = inventory.playerBattleItems.FirstOrDefault(x => x.id == inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].id).quantity;

                    itemQuantity[nItemSelected.Value].text = "x" + inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse.ToString();

                    if (inventory.teamItems[heroesManager.currenSelectedTeam].battleItems[nItemSelected.Value].quantityInUse == 0)
                        btnBattleItemsList[nItemSelected.Value].interactable = false;

                    heroTargetSelectedToHelp = null;

                    battleManagementButtons.ExitSelectedMode(false);
                    #endregion
                }

                #endregion
            }

            heroTargetSelectedToHelp = null;
        }
    }

    public void UpdateHPBars()
    {
        #region Hero Attack Enemy Bars

        for (int i = 0; i < heroList.Count; i++)
        {
            if (heroList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (enemyList.Where(x => x.statusBattle == StatusBattle.ALIVE).Count() == 0)
                {
                    board.currentState = GameState.win;
                    StartCoroutine(this.ShowWinPanel(showWinPanelSeconds));
                    break;
                }
                else
                {
                    if (heroList[i].enemyTarget.HasValue == true && enemyList[heroList[i].enemyTarget.Value].statusBattle == StatusBattle.ALIVE)
                        randomTarget = heroList[i].enemyTarget.GetValueOrDefault();
                    else
                    {
                        randomTarget = randomN.Next(3);
                        while (enemyList[randomTarget].statusBattle == StatusBattle.DEAD)
                        {
                            randomTarget = randomN.Next(3);
                        }

                        StartCoroutine(SetHeroTarget(i, randomTarget, heroList[i].enemyTarget));
                    }

                    randomDamage = randomN.Next(heroList[i].currentDmg, heroList[i].currentDmg);

                    if (!this.EnemyEvasionBlockCalc(i, randomTarget, randomN, heroList[i].elementColor) && randomDamage > 0)
                    {
                        randomTarget = this.EnemyThreatCalc(i, randomTarget, randomN, heroList[i].elementColor);
                        if (!this.EnemyEvasionBlockCalc(i, randomTarget, randomN, heroList[i].elementColor))
                        {
                            randomDamage = (enemyList[randomTarget].def - randomDamage);
                            if (randomDamage < 0)
                            {
                                StartCoroutine(ShowFloatTexting(imgHeroBattleList[i].gameObject, imgEnemyBattleList[randomTarget].gameObject, (randomDamage).ToString(), UIColorTemplate.DAMAGE, heroList[i].elementColor));
                                StartCoroutine(enemyBars.DecreaseEnemyHP((randomDamage * -1), randomTarget));
                                randomDamage = (enemyList[randomTarget].life + randomDamage) > 0 ? (enemyList[randomTarget].life + randomDamage) : 0;

                                if (randomDamage == 0)
                                {
                                    enemyList[randomTarget].statusBattle = StatusBattle.DEAD;
                                }

                                StartCoroutine(UpdateUIEnemyInfo(randomTarget, randomDamage));
                                enemyList[randomTarget].life = randomDamage;

                                if (enemyList.Where(x => x.statusBattle == StatusBattle.ALIVE).Count() == 0)
                                {
                                    board.currentState = GameState.win;
                                    StartCoroutine(this.ShowWinPanel(showWinPanelSeconds));
                                    break;
                                }
                            }
                            //else
                            //StartCoroutine(ShowFloatTexting(imgHeroBattleList[i].gameObject, imgEnemyBattleList[randomTarget].gameObject, "DEFEND.", UIColorTemplate.DEFENSE, heroList[i].elementColor));
                        }
                    }

                }
            }
        }

        heroList[0].currentDmg = 0;
        heroList[1].currentDmg = 0;
        heroList[2].currentDmg = 0;
        heroList[3].currentDmg = 0;

        #endregion

        #region Enemy Attack Hero Bars

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].statusBattle == StatusBattle.ALIVE)
            {
                randomTarget = randomN.Next(4);
                randomDamage = randomN.Next(enemyList[i].atk, (enemyList[i].atk + randomN.Next(5)));

                if (heroList.Where(x => x.statusBattle == StatusBattle.ALIVE).Count() > 0)
                {
                    while (heroList[randomTarget] == null || heroList[randomTarget].statusBattle == StatusBattle.DEAD)
                    {
                        randomTarget = randomN.Next(4);
                    }

                    if (!this.HeroEvasionBlockCalc(i, randomTarget, randomN, enemyList[i].elementColor))
                    {
                        randomTarget = this.HeroThreatCalc(i, randomTarget, randomN, enemyList[i].elementColor);
                        if (!this.HeroEvasionBlockCalc(i, randomTarget, randomN, enemyList[i].elementColor))
                        {
                            randomDamage = (heroList[randomTarget].def - randomDamage);
                            if (randomDamage < 0)
                            {
                                StartCoroutine(ShowFloatTexting(imgEnemyBattleList[i].gameObject, imgHeroBattleList[randomTarget].gameObject, (randomDamage).ToString(), UIColorTemplate.DAMAGE, enemyList[i].elementColor));
                                StartCoroutine(heroBars.DecreaseHeroHP((randomDamage * -1), randomTarget));
                                randomDamage = (heroList[randomTarget].life + randomDamage) > 0 ? (heroList[randomTarget].life + randomDamage) : 0;

                                if (randomDamage == 0)
                                {
                                    heroList[randomTarget].statusBattle = StatusBattle.DEAD;
                                    txtHeroLifeList[randomTarget].text = "HP:" + "0";
                                }

                                StartCoroutine(UpdateUIHeroInfo(randomTarget, randomDamage));
                                heroList[randomTarget].life = randomDamage;
                                txtHeroLifeList[randomTarget].text = "HP:" + randomDamage.ToString();

                                if (heroList.Where(x => x.statusBattle == StatusBattle.ALIVE).Count() == 0)
                                {
                                    board.currentState = GameState.lose;
                                    this.ShowLosePanel();
                                }
                            }
                            //else                            
                            //StartCoroutine(ShowFloatTexting(imgEnemyBattleList[i].gameObject, imgHeroBattleList[randomTarget].gameObject, "", UIColorTemplate.DEFENSE, enemyList[i].elementColor));
                            ///NOTE Need to do Defense Texting Float
                        }
                    }
                }
            }
        }

        #endregion

        nSequence = 0;

        if (board.currentState != GameState.lose && board.currentState != GameState.win)
            StartCoroutine(DelayAnimationToMove(waitTimebeforeMove));
    }

    public void UpdateResourceBars(List<string> dotMatches)
    {
        dotCount = 0;

        for (int i = 0; i < heroList.Count; i++)
        {
            if (dotMatches != null)
            {
                if (heroList[i] != null && heroList[i].statusBattle == StatusBattle.ALIVE)
                {
                    dotCount = dotMatches.Where(x => x.Contains((heroList[i].elementType.ToString()))).Count();

                    if (dotCount > 0)
                    {
                        heroBars.IncreaseHeroResource(dotCount, i);
                        heroList[i].currentDmg += dotCount * heroList[i].atk;
                    }
                    dotCount = 0;
                }
            }
        }
    }

    public int HeroThreatCalc(int nAttacker, int nTarget, System.Random randomN, Color32 elementColor)
    {
        //calcula para decidir qual herói leva o ataque - quanto mais Ameaça(AGGRO/tHREAT)  mais chance do herói ser atacado
        nTargetsList = null;
        nTargetsList = new List<int>();
        nThreat = 0;

        for (int i = 0; i < heroList.Count; i++)
        {
            nThreat = randomN.Next(0, heroList[i].threat);

            if (heroList[i].statusBattle == StatusBattle.ALIVE && nThreat > randomN.Next(0, (100 - heroList[i].threat)))
                nTargetsList.Add(nThreat);
            else
                nTargetsList.Add(0);
        }

        if (nTargetsList.Max() > 0 && nTargetsList.IndexOf(nTargetsList.Max()) != nTarget)
        {
            StartCoroutine(ShowFloatTexting(imgEnemyBattleList[nAttacker].gameObject, imgHeroBattleList[nTargetsList.IndexOf(nTargetsList.Max())].gameObject, ".", UIColorTemplate.AGGRO, elementColor));
            return nTargetsList.IndexOf(nTargetsList.Max());
        }
        else
            return nTarget;
    }

    public int EnemyThreatCalc(int nAttacker, int nTarget, System.Random randomN, Color32 elementColor)
    {
        //calcula para decidir qual Inimigo leva o ataque - quanto mais Ameaça(AGGRO/tHREAT)  mais chance do Inimigo ser atacado
        nTargetsList = null;
        nTargetsList = new List<int>();
        nThreat = 0;

        for (int i = 0; i < enemyList.Count; i++)
        {
            nThreat = randomN.Next(0, enemyList[i].threat);

            if (enemyList[i].statusBattle == StatusBattle.ALIVE && nThreat > randomN.Next(0, (100 - enemyList[i].threat)))
                nTargetsList.Add(nThreat);
            else
                nTargetsList.Add(0);

        }

        if (nTargetsList.Max() > 0 && nTargetsList.IndexOf(nTargetsList.Max()) != nTarget)
        {
            StartCoroutine(ShowFloatTexting(imgHeroBattleList[nAttacker].gameObject, imgEnemyBattleList[nTargetsList.IndexOf(nTargetsList.Max())].gameObject, ".", UIColorTemplate.AGGRO, elementColor));
            return nTargetsList.IndexOf(nTargetsList.Max());
        }
        else
            return nTarget;
    }

    public bool HeroEvasionBlockCalc(int nAttacker, int nTarget, System.Random randomN, Color32 elementColor)
    {
        isAtkBlockedOrEvaded = false;

        if (heroList[nTarget].block > 0 && randomN.Next(heroList[nTarget].block, 100) <= heroList[nTarget].block)
        {
            isAtkBlockedOrEvaded = true;
            StartCoroutine(ShowFloatTexting(imgEnemyBattleList[nAttacker].gameObject, imgHeroBattleList[nTarget].gameObject, "", UIColorTemplate.BLOCK, elementColor));
        }

        if (heroList[nTarget].evasion > 0 && randomN.Next(heroList[nTarget].evasion, 100) <= heroList[nTarget].evasion)
        {
            isAtkBlockedOrEvaded = true;
            StartCoroutine(ShowFloatTexting(imgEnemyBattleList[nAttacker].gameObject, imgHeroBattleList[nTarget].gameObject, "", UIColorTemplate.EVADE, elementColor));
        }

        return isAtkBlockedOrEvaded;
    }

    public bool EnemyEvasionBlockCalc(int nAttacker, int nTarget, System.Random randomN, Color32 elementColor)
    {
        isAtkBlockedOrEvaded = false;

        if (enemyList[nTarget].block > 0 && randomN.Next(enemyList[nTarget].block, 100) <= enemyList[nTarget].block)
        {
            isAtkBlockedOrEvaded = true;
            StartCoroutine(ShowFloatTexting(imgHeroBattleList[nAttacker].gameObject, imgEnemyBattleList[nTarget].gameObject, ".", UIColorTemplate.BLOCK, elementColor));
        }

        if (enemyList[nTarget].evasion > 0 && randomN.Next(enemyList[nTarget].evasion, 100) <= enemyList[nTarget].evasion)
        {
            isAtkBlockedOrEvaded = true;
            StartCoroutine(ShowFloatTexting(imgHeroBattleList[nAttacker].gameObject, imgEnemyBattleList[nTarget].gameObject, ".", UIColorTemplate.EVADE, elementColor));
        }

        return isAtkBlockedOrEvaded;
    }

    public IEnumerator ShowFloatTexting(GameObject objOrig, GameObject objDes, string text, UIColorTemplate uiColorText, Color32 elementColor)
    {
        if (board.currentState != GameState.itemTargetSelect && board.currentState != GameState.skillTargetSelect)
            nSequence++;
        else
            nSequence = 1;

        Vector3 newPos = new Vector3(objOrig.transform.position.x, objOrig.transform.position.y, 0f);

        if (board.currentState != GameState.itemTargetSelect && board.currentState != GameState.skillTargetSelect)
            waitTimebeforeMove += .2f;

        yield return new WaitForSeconds(nSequence * .3f);

        randomPos = objDes.transform.position;
        randomPos.x += UnityEngine.Random.Range(-.5f, .5f);
        randomPos.y += UnityEngine.Random.Range(-.5f, .5f);

        if (uiColorText == UIColorTemplate.DAMAGE || uiColorText == UIColorTemplate.HEAL || uiColorText == UIColorTemplate.RESOURCE || uiColorText == UIColorTemplate.RUNE)
        {
            if (uiColorText == UIColorTemplate.RUNE)
            {
                #region Rune Animation
                objDes.GetComponent<Animation>().Play("Battle-ReceiveBuffIndicator");

                var cloneFloatTextRune = Instantiate(floatingTextPrefab, randomPos, Quaternion.identity, objDes.transform);
                cloneFloatTextRune.GetComponent<TextMesh>().color = utilityTools.ReturnColor32(uiColorText);
                cloneFloatTextRune.GetComponent<TextMesh>().fontSize = 8;

                cloneFloatTextRune.GetComponent<TextMesh>().text = text;

                yield return new WaitForSeconds(.2f);

                Destroy(cloneFloatTextRune);

                #endregion 
            }
            else
            {
                objDes.GetComponent<Animation>().Play("Battle-ReceiveDamageIndicator");

                var newPsColor = damageEffectPrefab.GetComponent<ParticleSystem>().main;
                Color newColor = elementColor;
                newPsColor.startColor = newColor;

                var cloneDamageEffect = Instantiate(damageEffectPrefab, newPos, Quaternion.identity, this.transform); //GameBars Object
                cloneDamageEffect.GetComponent<TrailRenderer>().materials[0].color = elementColor;
                cloneDamageEffect.GetComponent<TrailRenderer>().materials[1].color = elementColor;

                cloneDamageEffectList.Add(cloneDamageEffect);
                newPosDamageEffectList.Add(randomPos);

                var cloneFloatText = Instantiate(floatingTextPrefab, randomPos, Quaternion.identity, objDes.transform);

                if (uiColorText == UIColorTemplate.RESOURCE)
                {
                    text += "%";
                    cloneFloatText.GetComponent<TextMesh>().color = elementColor;
                }
                else
                    cloneFloatText.GetComponent<TextMesh>().color = utilityTools.ReturnColor32(uiColorText);

                cloneFloatText.GetComponent<TextMesh>().text = text;

                yield return new WaitForSeconds(.2f);
                Destroy(cloneDamageEffect);
                cloneDamageEffectList.RemoveAt(0);
                newPosDamageEffectList.RemoveAt(0);

                yield return new WaitForSeconds(.2f);
                Destroy(cloneFloatText);

            }
        }
        else
        {
            GameObject dmgEffect = null;
            if (uiColorText != UIColorTemplate.AGGRO)
            {
                objDes.GetComponent<Animation>().Play();

                var newPsColor = damageEffectPrefab.GetComponent<ParticleSystem>().main;
                Color newColor = elementColor;
                newPsColor.startColor = newColor;

                var cloneDamageEffect = Instantiate(damageEffectPrefab, newPos, Quaternion.identity, this.transform); //GameBars Object
                cloneDamageEffect.GetComponent<TrailRenderer>().materials[0].color = elementColor;
                cloneDamageEffect.GetComponent<TrailRenderer>().materials[1].color = elementColor;
                cloneDamageEffectList.Add(cloneDamageEffect);
                newPosDamageEffectList.Add(randomPos);
                dmgEffect = cloneDamageEffect;
            }

            var cloneTextSprite = Instantiate(floatingTextPrefab, objDes.transform.position, Quaternion.identity, objDes.transform);
            cloneTextSprite.transform.GetChild(0).GetComponent<Image>().sprite = utilityTools.ReturnTextSprite(uiColorText);

            if (uiColorText != UIColorTemplate.AGGRO)
            {
                yield return new WaitForSeconds(.2f);
                Destroy(dmgEffect);
                cloneDamageEffectList.RemoveAt(0);
                newPosDamageEffectList.RemoveAt(0);
            }

            yield return new WaitForSeconds(.2f);
            Destroy(cloneTextSprite);
        }
    }

    public IEnumerator UpdateUIHeroInfo(int nTarget, int damageTaken)
    {
        yield return new WaitForSeconds(nSequence * 1f);
        //Função que da delay ao mudar as cores para dead

        if (damageTaken == 0)
        {
            yield return new WaitForSeconds(.1f);
            imgHeroBattleList[nTarget].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);
            heroBars.imgHPBars[nTarget].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);
            heroBars.imgBkgHPBars[nTarget].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);
        }

        txtLifeHeroes[nTarget].text = damageTaken.ToString();
    }

    public IEnumerator UpdateUIEnemyInfo(int nTarget, int damageTaken)
    {
        yield return new WaitForSeconds(nSequence * 1f);
        //Função que da delay ao mudar as cores para dead

        if (damageTaken == 0)
        {
            //yield return new WaitForSeconds(.1f);
            imgEnemyBattleList[nTarget].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);
            enemyBars.imgHPBars[nTarget].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);
            enemyBars.imgBkgHPBars[nTarget].color = utilityTools.ReturnColor32(UIColorTemplate.NO_INTERACTABLE);
        }

        txtLifeEnemys[nTarget].text = damageTaken.ToString();
    }

    public void SetDefaultColorsSelectedMode()
    {
        //coloca a cor de interactable para quando sair do modo de seleção de alvo
        for (int i = 0; i < heroList.Count; i++)
        {
            if (i < 3 && enemyList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (imgEnemyBattleList[i] != null)
                    imgEnemyBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (enemyBars.imgHPBars[i] != null)
                    enemyBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (enemyBars.imgBkgHPBars[i] != null)
                    enemyBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
            }

            if (heroList[i].statusBattle == StatusBattle.ALIVE)
            {
                if (imgHeroBattleList[i] != null)
                    imgHeroBattleList[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (heroBars.imgHPBars[i] != null)
                    heroBars.imgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);

                if (heroBars.imgBkgHPBars[i] != null)
                    heroBars.imgBkgHPBars[i].color = utilityTools.ReturnColor32(UIColorTemplate.INTERACTABLE);
            }
        }
    }

    public IEnumerator DelayAnimationToMove(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        waitTimebeforeMove = 0f;
        board.currentState = GameState.move;
    }

    #region Options Menu
    public void OpenOptions()
    {
        if (board.currentState == GameState.move)
        {
            board.currentState = GameState.inOptionsMenu;
            optionMenu.SetActive(true);
            btnCloseOptions.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        optionMenu.SetActive(false);
        btnCloseOptions.SetActive(false);
        btnExitCancel.SetActive(false);
        btnExitConfirm.SetActive(false);
        txtExitBattle.SetActive(false);
        btnExitBattle.SetActive(true);
        board.currentState = GameState.move;
    }

    public void OpenValidateExit()
    {
        txtExitBattle.SetActive(true);
        btnExitBattle.SetActive(false);
        btnExitCancel.SetActive(true);
        btnExitConfirm.SetActive(true);
    }

    public void ConfirmationExit(bool confirm)
    {
        if (confirm == true)
        {
            board.currentState = GameState.lose;
            optionMenu.SetActive(false);
            btnCloseOptions.SetActive(false);
            btnExitCancel.SetActive(false);
            btnExitConfirm.SetActive(false);
            txtExitBattle.SetActive(false);
            btnExitBattle.SetActive(true);
            mapMainMenu.OpenMazeMap();
        }
        else
        {
            this.CloseOptions();
        }
    }

    #endregion

    #region Win/Lose Panels

    public IEnumerator ShowWinPanel(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        losePanel.SetActive(false);
        winPanel.SetActive(true);
        scrollRectDrops.horizontalNormalizedPosition = 0f;

        if (levelManager.levelsContainer.currentLevelOpen != null)
            nItems = levelManager.levelsContainer.currentLevelOpen.levelDropItems.Count;
        else
            nItems = 0;

        txtPlayerGold.text = inventory.GetComponent<PlayerProfile>().gold.ToString();
        inventory.GetComponent<PlayerProfile>().gold = (inventory.GetComponent<PlayerProfile>().gold + levelManager.levelsContainer.currentLevelOpen.levelGoldDrop);
        inventory.GetComponent<PlayerProfile>().RefreshPlayerGoldInfo();
        txtGoldReceived.text = levelManager.levelsContainer.currentLevelOpen.levelGoldDrop.ToString();

        for (int i = 0; i < nItems; i++)
        {
            baseDropItem = null;
            baseDropItem = levelManager.levelsContainer.currentLevelOpen.levelDropItems[i];

            switch (baseDropItem.itemType)
            {
                case ItemType.Exp:
                    if (inventory.playerExpItems.Find(x => x.id == baseDropItem.id) == null)
                    {
                        inventory.playerExpItems.Add(
                            new DropItem
                            {
                                id = baseDropItem.id,
                                idInInventory = inventory.playerExpItems.Count + 1,
                                quantity = baseDropItem.quantity,
                                quantityInUse = baseDropItem.quantityInUse,
                                quantityMaxForDrop = baseDropItem.quantityMaxForDrop,
                                containedExp = baseDropItem.containedExp,
                                name = baseDropItem.name,
                                itemType = baseDropItem.itemType,
                                elementType = baseDropItem.elementType,
                                infoItem = baseDropItem.infoItem,
                                spriteNormalSize = baseDropItem.spriteNormalSize,
                                spriteMiniSize = baseDropItem.spriteMiniSize
                            }
                        );
                    }
                    else
                    {
                        inventory.playerExpItems.FirstOrDefault(x => x.id == levelManager.levelsContainer.currentLevelOpen.levelDropItems[i].id).quantity += baseDropItem.quantity;
                    }
                    break;
                default:
                    // NadaFAz x_x
                    break;
            }
        }

        //Aplica o Status de fase finalizada com sucesso ao currentLevelOpen
        levelManager.UpdateLevelOnWin(LevelStatus.CLEARED);
    }

    public void ShowLosePanel()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }
    #endregion

    public class HeroBars : MonoBehaviour
    {
        public List<Image> imgHPBars;
        public List<Image> imgBkgHPBars;
        public List<Image> imgResourceBars;
        public List<Image> imgBkgResourceBars;
        public List<float> fillHPBars; //Valor calculado de quanto será retirado da barra (diminuir/acrescentar HP , por ponto deHP)
        public List<Image> imgSkillStatus;
        private BattleManagement battleManagement;
        private BattleManagementButtons battleManagementButtons;

        #region Load Img Bars From Folder
        public List<Sprite> spriteBkgResourceBarList;
        public List<Sprite> spriteResourceBarList;
        public List<Sprite> spriteSkillBarList;
        #endregion

        #region Animation
        private List<float> heroHealList;
        private List<float> heroOldLife;
        private List<float> enemyDamageList;
        private float fillHP_0;
        private float fillHP_1;
        private float fillHP_2;
        private float fillHP_3;
        private List<float> resourceListToFill;
        private float fillRes_0;
        private float fillRes_1;
        private float fillRes_2;
        private float fillRes_3;
        private List<float> resourceListToUnFill;
        #endregion

        private double resPercent;

        #region Hero Bars
        public void Start()
        {
            battleManagementButtons = FindFirstObjectByType<BattleManagementButtons>();
            enemyDamageList = new List<float>() { 0, 0, 0, 0 };
            heroHealList = new List<float>() { 0, 0, 0, 0 };
            heroOldLife = new List<float>() { 0, 0, 0, 0 };
            resourceListToFill = new List<float>() { 0, 0, 0, 0 };
            resourceListToUnFill = new List<float>() { 0, 0, 0, 0 };
            battleManagement = FindFirstObjectByType<BattleManagement>();
            imgHPBars = new List<Image>();
            imgBkgHPBars = new List<Image>();
            imgResourceBars = new List<Image>();
            imgBkgResourceBars = new List<Image>();
            imgSkillStatus = new List<Image>();
            spriteBkgResourceBarList = new List<Sprite>();
            spriteResourceBarList = new List<Sprite>();
            spriteSkillBarList = new List<Sprite>();

            #region Add Img Bars To List 

            #region Background HP Bars

            if (GameObject.FindWithTag("BkgHPheroBar1") != null)
                imgBkgHPBars.Add(GameObject.FindWithTag("BkgHPheroBar1").GetComponent<Image>());
            else
                imgBkgHPBars.Add(null);

            if (GameObject.FindWithTag("BkgHPheroBar2") != null)
                imgBkgHPBars.Add(GameObject.FindWithTag("BkgHPheroBar2").GetComponent<Image>());
            else
                imgBkgHPBars.Add(null);

            if (GameObject.FindWithTag("BkgHPheroBar3") != null)
                imgBkgHPBars.Add(GameObject.FindWithTag("BkgHPheroBar3").GetComponent<Image>());
            else
                imgBkgHPBars.Add(null);

            if (GameObject.FindWithTag("BkgHPheroBar4") != null)
                imgBkgHPBars.Add(GameObject.FindWithTag("BkgHPheroBar4").GetComponent<Image>());
            else
                imgBkgHPBars.Add(null);

            #endregion

            #region HP bars

            if (GameObject.FindWithTag("HPheroBar1") != null)
                imgHPBars.Add(GameObject.FindWithTag("HPheroBar1").GetComponent<Image>());
            else
                imgHPBars.Add(null);

            if (GameObject.FindWithTag("HPheroBar2") != null)
                imgHPBars.Add(GameObject.FindWithTag("HPheroBar2").GetComponent<Image>());
            else
                imgHPBars.Add(null);

            if (GameObject.FindWithTag("HPheroBar3") != null)
                imgHPBars.Add(GameObject.FindWithTag("HPheroBar3").GetComponent<Image>());
            else
                imgHPBars.Add(null);

            if (GameObject.FindWithTag("HPheroBar4") != null)
                imgHPBars.Add(GameObject.FindWithTag("HPheroBar4").GetComponent<Image>());
            else
                imgHPBars.Add(null);

            #endregion

            #region Background ResourceBars

            if (GameObject.FindWithTag("BkgResourceHeroBar1") != null)
                imgBkgResourceBars.Add(GameObject.FindWithTag("BkgResourceHeroBar1").GetComponent<Image>());
            else
                imgBkgResourceBars.Add(null);

            if (GameObject.FindWithTag("BkgResourceHeroBar2") != null)
                imgBkgResourceBars.Add(GameObject.FindWithTag("BkgResourceHeroBar2").GetComponent<Image>());
            else
                imgBkgResourceBars.Add(null);

            if (GameObject.FindWithTag("BkgResourceHeroBar3") != null)
                imgBkgResourceBars.Add(GameObject.FindWithTag("BkgResourceHeroBar3").GetComponent<Image>());
            else
                imgBkgResourceBars.Add(null);

            if (GameObject.FindWithTag("BkgResourceHeroBar4") != null)
                imgBkgResourceBars.Add(GameObject.FindWithTag("BkgResourceHeroBar4").GetComponent<Image>());
            else
                imgBkgResourceBars.Add(null);

            #endregion

            #region ResourceBars

            if (GameObject.FindWithTag("ResourceHeroBar1") != null)
                imgResourceBars.Add(GameObject.FindWithTag("ResourceHeroBar1").GetComponent<Image>());
            else
                imgResourceBars.Add(null);

            if (GameObject.FindWithTag("ResourceHeroBar2") != null)
                imgResourceBars.Add(GameObject.FindWithTag("ResourceHeroBar2").GetComponent<Image>());
            else
                imgResourceBars.Add(null);

            if (GameObject.FindWithTag("ResourceHeroBar3") != null)
                imgResourceBars.Add(GameObject.FindWithTag("ResourceHeroBar3").GetComponent<Image>());
            else
                imgResourceBars.Add(null);

            if (GameObject.FindWithTag("ResourceHeroBar4") != null)
                imgResourceBars.Add(GameObject.FindWithTag("ResourceHeroBar4").GetComponent<Image>());
            else
                imgResourceBars.Add(null);

            #endregion

            #region Skill Bar Icon
            if (GameObject.FindWithTag("SkillHeroStatus1") != null)
                imgSkillStatus.Add(GameObject.FindWithTag("SkillHeroStatus1").GetComponent<Image>());
            else
                imgSkillStatus.Add(null);

            if (GameObject.FindWithTag("SkillHeroStatus2") != null)
                imgSkillStatus.Add(GameObject.FindWithTag("SkillHeroStatus2").GetComponent<Image>());
            else
                imgSkillStatus.Add(null);

            if (GameObject.FindWithTag("SkillHeroStatus3") != null)
                imgSkillStatus.Add(GameObject.FindWithTag("SkillHeroStatus3").GetComponent<Image>());
            else
                imgSkillStatus.Add(null);

            if (GameObject.FindWithTag("SkillHeroStatus4") != null)
                imgSkillStatus.Add(GameObject.FindWithTag("SkillHeroStatus4").GetComponent<Image>());
            else
                imgSkillStatus.Add(null);
            #endregion

            #region Img From Folder

            spriteBkgResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/BkgResource-FIRE"));
            spriteBkgResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/BkgResource-NATURE"));
            spriteBkgResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/BkgResource-ICE"));
            spriteBkgResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/BkgResource-SHADOW"));
            spriteBkgResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/BkgResource-HOLY"));
            spriteBkgResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/BkgResource-PHYSICAL"));

            spriteResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/Resource-FIRE"));
            spriteResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/Resource-NATURE"));
            spriteResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/Resource-ICE"));
            spriteResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/Resource-SHADOW"));
            spriteResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/Resource-HOLY"));
            spriteResourceBarList.Add(Resources.Load<Sprite>("Art/Bars/Resource-PHYSICAL"));

            spriteSkillBarList.Add(Resources.Load<Sprite>("Art/Bars/SkillStatus-FIRE"));
            spriteSkillBarList.Add(Resources.Load<Sprite>("Art/Bars/SkillStatus-NATURE"));
            spriteSkillBarList.Add(Resources.Load<Sprite>("Art/Bars/SkillStatus-ICE"));
            spriteSkillBarList.Add(Resources.Load<Sprite>("Art/Bars/SkillStatus-SHADOW"));
            spriteSkillBarList.Add(Resources.Load<Sprite>("Art/Bars/SkillStatus-HOLY"));
            spriteSkillBarList.Add(Resources.Load<Sprite>("Art/Bars/SkillStatus-PHYSICAL"));

            #endregion

            #endregion

            for (int i = 0; i < 4; i++)
            {
                if (imgHPBars[i] != null)
                {
                    imgHPBars[i].fillAmount = 1f;
                    imgResourceBars[i].fillAmount = 0f;
                }
            }
        }

        void Update()
        {
            #region Decrease HP        

            if (imgHPBars[0] != null && Math.Round(imgHPBars[0].fillAmount, 2) > Math.Round(1f - (enemyDamageList[0] - heroHealList[0]), 2))
            {
                fillHP_0 = Time.deltaTime * (enemyDamageList[0] / 2);
                imgHPBars[0].fillAmount -= fillHP_0;

                if (imgHPBars[0].fillAmount <= 0)
                {
                    imgHPBars[0].fillAmount = 0;
                    enemyDamageList[0] = 0;
                    heroHealList[0] = 0;
                }
            }

            if (imgHPBars[1] != null && Math.Round(imgHPBars[1].fillAmount, 2) > Math.Round(1f - (enemyDamageList[1] - heroHealList[1]), 2))
            {
                fillHP_1 = Time.deltaTime * (enemyDamageList[1] / 2);
                imgHPBars[1].fillAmount -= fillHP_1;

                if (imgHPBars[1].fillAmount <= 0)
                {
                    imgHPBars[1].fillAmount = 0;
                    enemyDamageList[1] = 0;
                    heroHealList[1] = 0;
                }
            }

            if (imgHPBars[2] != null && Math.Round(imgHPBars[2].fillAmount, 2) > Math.Round(1f - (enemyDamageList[2] - heroHealList[2]), 2))
            {
                fillHP_2 = Time.deltaTime * (enemyDamageList[2] / 2);
                imgHPBars[2].fillAmount -= fillHP_2;

                if (imgHPBars[2].fillAmount <= 0)
                {
                    imgHPBars[2].fillAmount = 0;
                    enemyDamageList[2] = 0;
                    heroHealList[2] = 0;
                }
            }

            if (imgHPBars[3] != null && Math.Round(imgHPBars[3].fillAmount, 2) > Math.Round(1f - (enemyDamageList[3] - heroHealList[3]), 2))
            {
                fillHP_3 = Time.deltaTime * (enemyDamageList[3] / 2);
                imgHPBars[3].fillAmount -= fillHP_3;

                if (imgHPBars[3].fillAmount <= 0)
                {
                    imgHPBars[3].fillAmount = 0;
                    enemyDamageList[3] = 0;
                    heroHealList[3] = 0;
                }
            }
            #endregion

            #region Increase HP 

            if (imgHPBars[0] != null && Math.Round(imgHPBars[0].fillAmount, 2) < Math.Round(1f - (enemyDamageList[0] - heroHealList[0]), 2))
            {
                fillHP_0 = Time.deltaTime * (heroHealList[0] / 2);
                imgHPBars[0].fillAmount += fillHP_0;

                if (imgHPBars[0].fillAmount >= 1f)
                {
                    heroOldLife[0] = 0;
                    imgHPBars[0].fillAmount = 1;
                    enemyDamageList[0] = 0;
                    heroHealList[0] = 0;
                }
            }

            if (imgHPBars[1] != null && Math.Round(imgHPBars[1].fillAmount, 2) < Math.Round(1f - (enemyDamageList[1] - heroHealList[1]), 2))
            {
                fillHP_1 = Time.deltaTime * (heroHealList[1]);
                imgHPBars[1].fillAmount += fillHP_1;

                if (imgHPBars[1].fillAmount >= 1f)
                {
                    heroOldLife[1] = 0;
                    imgHPBars[1].fillAmount = 1;
                    enemyDamageList[1] = 0;
                    heroHealList[1] = 0;
                }
            }

            if (imgHPBars[2] != null && Math.Round(imgHPBars[2].fillAmount, 2) < Math.Round(1f - (enemyDamageList[2] - heroHealList[2]), 2))
            {
                fillHP_2 = Time.deltaTime * (heroHealList[2]);
                imgHPBars[2].fillAmount += fillHP_2;

                if (imgHPBars[2].fillAmount >= 1f)
                {
                    heroOldLife[2] = 0;
                    imgHPBars[2].fillAmount = 1;
                    enemyDamageList[2] = 0;
                    heroHealList[2] = 0;
                }
            }

            if (imgHPBars[3] != null && Math.Round(imgHPBars[3].fillAmount, 2) < Math.Round(1f - (enemyDamageList[3] - heroHealList[3]), 2))
            {
                fillHP_3 = Time.deltaTime * (heroHealList[3]);
                imgHPBars[3].fillAmount += fillHP_3;

                if (imgHPBars[3].fillAmount >= 1f)
                {
                    heroOldLife[3] = 0;
                    imgHPBars[3].fillAmount = 1;
                    enemyDamageList[3] = 0;
                    heroHealList[3] = 0;
                }
            }
            #endregion

            #region Increase/Decrease Resource

            if (imgResourceBars[0] != null && imgResourceBars[0].fillAmount < (0f + resourceListToFill[0]))
            {
                fillRes_0 = Time.deltaTime * (resourceListToFill[0]);
                imgResourceBars[0].fillAmount += fillRes_0;

                if (imgResourceBars[0].fillAmount >= 1)
                {
                    imgResourceBars[0].fillAmount = 1f;
                    resourceListToFill[0] = 0f;
                    imgSkillStatus[0].color = new Color32(255, 255, 255, 255); //Interactable
                    imgSkillStatus[0].gameObject.transform.GetChild(0).gameObject.SetActive(true);

                    battleManagement.skillIndicatorEffectPrefabs[0].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[1].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[2].gameObject.SetActive(true);

                    if (battleManagement.heroList[0].skillHero.isSkillAvailable == false)
                    {
                        battleManagement.heroList[0].skillHero.isSkillAvailable = true;
                        battleManagementButtons.btnHeroActiveSkillList[0].gameObject.SetActive(true);
                    }
                }
            }

            if (imgResourceBars[0] != null && resourceListToUnFill[0] > 0 && imgResourceBars[0].fillAmount > 0)
            {
                fillRes_0 = Time.deltaTime * (resourceListToUnFill[0]);
                imgResourceBars[0].fillAmount -= fillRes_0;

                if (imgResourceBars[0].fillAmount <= 0)
                {
                    imgResourceBars[0].fillAmount = 0f;
                    resourceListToUnFill[0] = 0f;
                    imgSkillStatus[0].color = new Color32(55, 55, 55, 255); //No Interactable
                    imgSkillStatus[0].gameObject.transform.GetChild(0).gameObject.SetActive(false);

                    battleManagement.skillIndicatorEffectPrefabs[0].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[1].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[2].gameObject.SetActive(false);

                    battleManagement.heroList[0].skillHero.isSkillAvailable = false;
                    battleManagementButtons.btnHeroActiveSkillList[0].gameObject.SetActive(false);
                }
            }


            if (imgResourceBars[1] != null && imgResourceBars[1].fillAmount < (0f + resourceListToFill[1]))
            {
                fillRes_1 = Time.deltaTime * (resourceListToFill[1]);
                imgResourceBars[1].fillAmount += fillRes_1;

                if (imgResourceBars[1].fillAmount >= 1)
                {
                    imgResourceBars[1].fillAmount = 1f;
                    resourceListToFill[1] = 0f;
                    imgSkillStatus[1].color = new Color32(255, 255, 255, 255); //Interactable
                    imgSkillStatus[1].gameObject.transform.GetChild(0).gameObject.SetActive(true);

                    battleManagement.skillIndicatorEffectPrefabs[3].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[4].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[5].gameObject.SetActive(true);

                    if (battleManagement.heroList[1].skillHero.isSkillAvailable == false)
                    {
                        battleManagement.heroList[1].skillHero.isSkillAvailable = true;
                        battleManagementButtons.btnHeroActiveSkillList[1].gameObject.SetActive(true);
                    }
                }
            }

            if (imgResourceBars[1] != null && resourceListToUnFill[1] > 0 && imgResourceBars[1].fillAmount > 0)
            {
                fillRes_1 = Time.deltaTime * (resourceListToUnFill[1]);
                imgResourceBars[1].fillAmount -= fillRes_1;

                if (imgResourceBars[1].fillAmount <= 0)
                {
                    imgResourceBars[1].fillAmount = 0f;
                    resourceListToUnFill[1] = 0f;
                    imgSkillStatus[1].color = new Color32(55, 55, 55, 255); //No Interactable
                    imgSkillStatus[1].gameObject.transform.GetChild(0).gameObject.SetActive(false);

                    battleManagement.skillIndicatorEffectPrefabs[3].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[4].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[5].gameObject.SetActive(false);

                    battleManagement.heroList[1].skillHero.isSkillAvailable = false;
                    battleManagementButtons.btnHeroActiveSkillList[1].gameObject.SetActive(false);
                }
            }


            if (imgResourceBars[2] != null && imgResourceBars[2].fillAmount < (0f + resourceListToFill[2]))
            {
                fillRes_2 = Time.deltaTime * (resourceListToFill[2]);
                imgResourceBars[2].fillAmount += fillRes_2;

                if (imgResourceBars[2].fillAmount >= 1)
                {
                    imgResourceBars[2].fillAmount = 1f;
                    resourceListToFill[2] = 0f;
                    imgSkillStatus[2].color = new Color32(255, 255, 255, 255); //Interactable
                    imgSkillStatus[2].gameObject.transform.GetChild(0).gameObject.SetActive(true);

                    battleManagement.skillIndicatorEffectPrefabs[6].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[7].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[8].gameObject.SetActive(true);

                    if (battleManagement.heroList[2].skillHero.isSkillAvailable == false)
                    {
                        battleManagement.heroList[2].skillHero.isSkillAvailable = true;
                        battleManagementButtons.btnHeroActiveSkillList[2].gameObject.SetActive(true);
                    }
                }
            }

            if (imgResourceBars[2] != null && resourceListToUnFill[2] > 0 && imgResourceBars[2].fillAmount > 0)
            {
                fillRes_2 = Time.deltaTime * (resourceListToUnFill[2]);
                imgResourceBars[2].fillAmount -= fillRes_2;

                if (imgResourceBars[2].fillAmount <= 0)
                {
                    imgResourceBars[2].fillAmount = 0f;
                    resourceListToUnFill[2] = 0f;
                    imgSkillStatus[2].color = new Color32(55, 55, 55, 255); //No Interactable
                    imgSkillStatus[2].gameObject.transform.GetChild(0).gameObject.SetActive(false);

                    battleManagement.skillIndicatorEffectPrefabs[6].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[7].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[8].gameObject.SetActive(false);

                    battleManagement.heroList[2].skillHero.isSkillAvailable = false;
                    battleManagementButtons.btnHeroActiveSkillList[2].gameObject.SetActive(false);
                }
            }


            if (imgResourceBars[3] != null && imgResourceBars[3].fillAmount < (0f + resourceListToFill[3]))
            {
                fillRes_3 = Time.deltaTime * (resourceListToFill[3]);
                imgResourceBars[3].fillAmount += fillRes_3;

                if (imgResourceBars[3].fillAmount >= 1)
                {
                    imgResourceBars[3].fillAmount = 1f;
                    resourceListToFill[3] = 0f;
                    imgSkillStatus[3].color = new Color32(255, 255, 255, 255); //Interactable
                    imgSkillStatus[3].gameObject.transform.GetChild(0).gameObject.SetActive(true);

                    battleManagement.skillIndicatorEffectPrefabs[9].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[10].gameObject.SetActive(true);
                    battleManagement.skillIndicatorEffectPrefabs[11].gameObject.SetActive(true); ;

                    if (battleManagement.heroList[3].skillHero.isSkillAvailable == false)
                    {
                        battleManagement.heroList[3].skillHero.isSkillAvailable = true;
                        battleManagementButtons.btnHeroActiveSkillList[3].gameObject.SetActive(true);
                    }
                }
            }

            if (imgResourceBars[3] != null && resourceListToUnFill[3] > 0 && imgResourceBars[3].fillAmount > 0)
            {
                fillRes_3 = Time.deltaTime * (resourceListToUnFill[3]);
                imgResourceBars[3].fillAmount -= fillRes_3;

                if (imgResourceBars[3].fillAmount <= 0)
                {
                    imgResourceBars[3].fillAmount = 0f;
                    resourceListToUnFill[3] = 0f;
                    imgSkillStatus[3].color = new Color32(55, 55, 55, 255); //No Interactable
                    imgSkillStatus[3].gameObject.transform.GetChild(0).gameObject.SetActive(false);

                    battleManagement.skillIndicatorEffectPrefabs[9].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[10].gameObject.SetActive(false);
                    battleManagement.skillIndicatorEffectPrefabs[11].gameObject.SetActive(false);

                    battleManagement.heroList[3].skillHero.isSkillAvailable = false;
                    battleManagementButtons.btnHeroActiveSkillList[3].gameObject.SetActive(false);
                }
            }
            #endregion
        }

        public IEnumerator DecreaseHeroHP(int dmg, int nHero)
        {
            yield return new WaitForSeconds(battleManagement.nSequence * 1f);
            enemyDamageList[nHero] += ((dmg * fillHPBars[nHero]) / 100f);
        }

        public IEnumerator IncreaseHeroHP(int heal, int nHero)
        {
            yield return new WaitForSeconds(battleManagement.nSequence * 1f);
            heroOldLife[nHero] = imgHPBars[nHero].fillAmount;
            heroHealList[nHero] += ((heal * fillHPBars[nHero]) / 100f);
        }

        public void ResetHeroHP()
        {
            //imgHPBar1.fillAmount = 1
        }

        public void IncreaseHeroResource(int var, int nHero)
        {
            //Deixar 10 // 10 gemas igual a 100% de Mana
            if (imgResourceBars[nHero].fillAmount < 1f)
            {
                resourceListToFill[nHero] += (var * 10) / 100f;
                resPercent = Math.Round(imgResourceBars[nHero].fillAmount + resourceListToFill[nHero] * 100);

                if (resPercent > 100)
                    resPercent = 100;

                battleManagement.txtResourceList[nHero].text = "RES:" + resPercent + "%";
            }
        }

        public void DecreaseHeroResource(int nHero)
        {
            resourceListToUnFill[nHero] = 2f;
        }
        #endregion
    }
}