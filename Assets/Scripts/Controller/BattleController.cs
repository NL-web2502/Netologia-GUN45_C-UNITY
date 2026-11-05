using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BattleController : MonoBehaviour
{
    [SerializeField] private Battlefield battlefield;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera mainCamera;

    public Battlefield Battlefield => battlefield;

    private GameStatus _status = GameStatus.Wait;
    private Team _currentTeam = Team.White;
    private Unit _selectedUnit;
    private List<MoveValidator.Move> _validMoves = new();

    private bool _isCaptureMandatory;
    private List<Unit> _captureUnits = new();
    private bool _isRecursiveCapture;

    private InputActionAssets _inputActions;  
    private InputAction _selectAction;
    private InputAction _confirmAction;
    private InputAction _cancelAction;

    private void Awake()
    {
       
        _inputActions = new InputActionAssets(); 
         
        _selectAction = _inputActions.Gameplay.Select;
        _confirmAction = _inputActions.Gameplay.Confirm;
        _cancelAction = _inputActions.Gameplay.Cancel;
    }

    private void OnEnable()
    {
        _selectAction.performed += OnSelectPerformed;
        _confirmAction.performed += OnConfirmPerformed;
        _cancelAction.performed += OnCancelPerformed;
        _selectAction.Enable();
        _confirmAction.Enable();
        _cancelAction.Enable();
    }

    private void OnDisable()
    {
        _selectAction.Disable();
        _confirmAction.Disable();
        _cancelAction.Disable();
    }

    private void Start()
    {
        battlefield.GenerateBoard(this);
        StartTurn(Team.White);
    }
 
    public void OnCellClicked(Cell cell)
    {
        if (_status != GameStatus.Move || _selectedUnit == null) return;
    }

    public void OnUnitClicked(Unit unit)
    {
        if (_status != GameStatus.Select) return;
        if (unit.Team != _currentTeam) return;

        if (_isCaptureMandatory && !_captureUnits.Contains(unit))
        {
            Debug.LogWarning("Сначала нужно выполнить обязательное взятие!");
            return;
        }

        ClearHighlights();
        _selectedUnit = unit;
        _status = GameStatus.Move;
        _validMoves = MoveValidator.GetMoves(unit, battlefield);

        foreach (var m in _validMoves)
            m.To.SetHighlight(false, false, true);
    }
 
    private void OnSelectPerformed(InputAction.CallbackContext ctx) { /* клик идёт через EventSystem */ }

    private void OnConfirmPerformed(InputAction.CallbackContext ctx)
    {
        if (_status != GameStatus.Move || _selectedUnit == null) return;

        Cell target = GetCellUnderPointer();
        if (target == null) return;

        var move = FindMove(_selectedUnit.CurrentCell, target);
        if (move.To == null) return;

        new MoveCommand(this, _selectedUnit).Interact(target);
    }

    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        if (_isRecursiveCapture)
        {
            Debug.LogWarning("Рекурсивное взятие нельзя отменить — продолжайте бить!");
            return;
        }
        if (_status == GameStatus.Move)
        {
            _selectedUnit = null;
            _validMoves.Clear();
            _status = GameStatus.Select;
            ClearHighlights();
            HighlightMandatoryUnits();
        }
    }
 
    public MoveValidator.Move FindMove(Cell from, Cell to)
    {
        foreach (var m in _validMoves)
            if (m.From == from && m.To == to) return m;
        return default;
    }

    public void StartRecursiveCapture(Unit unit)
    {
        _isRecursiveCapture = true;
        _selectedUnit = unit;
        _status = GameStatus.Move;
        _validMoves = MoveValidator.GetMoves(unit, battlefield, capturesOnly: true);

        foreach (var cell in battlefield.AllCells) cell.SetHighlight(false, false, false);
        unit.CurrentCell.SetHighlight(false, true, false);
        foreach (var m in _validMoves) m.To.SetHighlight(false, false, true);
    }

    public void FinishTurn()
    {
        _isRecursiveCapture = false;
        _selectedUnit = null;
        _validMoves.Clear();
        _status = GameStatus.Select;

        _currentTeam = _currentTeam == Team.White ? Team.Black : Team.White;
        playerController?.OnTurnChanged(_currentTeam);

        ClearHighlights();
        AnalyzeMandatoryCapture();
        HighlightMandatoryUnits();
    }

    private void StartTurn(Team team)
    {
        _currentTeam = team;
        _status = GameStatus.Select;
        playerController?.OnTurnChanged(team);
        AnalyzeMandatoryCapture();
        HighlightMandatoryUnits();
    }
 
    private void AnalyzeMandatoryCapture()
    {
        _isCaptureMandatory = false;
        _captureUnits.Clear();

        foreach (var cell in battlefield.AllCells)
        {
            Unit u = cell.OccupiedUnit;
            if (u == null || u.Team != _currentTeam) continue;
            if (MoveValidator.HasMoreCaptures(u, battlefield))
            {
                _isCaptureMandatory = true;
                _captureUnits.Add(u);
            }
        }
    }

    private void HighlightMandatoryUnits()
    {
        if (!_isCaptureMandatory) return;
        foreach (var u in _captureUnits)
            u.CurrentCell.SetHighlight(false, true, false);
    }
 
    public void OnCellHover(Cell cell, bool enter)
    {
        if (enter) cell.SetHighlight(true, false, false);
        else cell.SetHighlight(false, false, false);
    }

    public void OnUnitHover(Unit unit, bool enter)
    {
        if (enter) unit.CurrentCell.SetHighlight(true, false, false);
        else unit.CurrentCell.SetHighlight(false, false, false);
    }

    private void ClearHighlights()
    {
        foreach (var cell in battlefield.AllCells)
            cell.SetHighlight(false, false, false);
    }

    private Cell GetCellUnderPointer()
    {
        if (mainCamera == null) return null;
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var cell = hit.collider.GetComponent<Cell>();
            if (cell != null) return cell;

            var unit = hit.collider.GetComponent<Unit>();
            if (unit != null) return unit.CurrentCell;
        }
        return null;
    }
}