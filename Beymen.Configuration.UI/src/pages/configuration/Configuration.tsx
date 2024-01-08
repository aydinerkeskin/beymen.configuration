import * as React from 'react';
import Container from '@mui/material/Container';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/DeleteOutlined';
import SaveIcon from '@mui/icons-material/Save';
import CancelIcon from '@mui/icons-material/Close';
import {
  GridRowsProp,
  GridRowModesModel,
  GridRowModes,
  DataGrid,
  GridColDef,
  GridToolbarContainer,
  GridActionsCellItem,
  GridEventListener,
  GridRowId,
  GridRowModel,
  GridRowEditStopReasons,
} from '@mui/x-data-grid';
import {
    randomId
  } from '@mui/x-data-grid-generator';
import { useEffect } from "react";

let initialRows: GridRowsProp = [
    {
      id: randomId(),
      applicationName: "SERVICE-A",
      name: "SiteName",
      type: "String",
      value: "beymen.com.tr",
      isActive: "Active"
    },
  ];

  interface EditToolbarProps {
    setRows: (newRows: (oldRows: GridRowsProp) => GridRowsProp) => void;
    setRowModesModel: (
      newModel: (oldModel: GridRowModesModel) => GridRowModesModel,
    ) => void;
  }
  
  function EditToolbar(props: EditToolbarProps) {
    const { setRows, setRowModesModel } = props;
  
    const handleClick = () => {
      const id = randomId();
      setRows((oldRows) => [...oldRows, { id, applicationName: '', name: '', type: 'String', value: '', isActive: 'Active', isNewRecord: true }]);
      setRowModesModel((oldModel) => ({
        ...oldModel,
        [id]: { mode: GridRowModes.Edit, fieldToFocus: 'applicationName', type: "new" },
      }));
    };
  
    return (
      <GridToolbarContainer>
        <Button color="primary" startIcon={<AddIcon />} onClick={handleClick}>
          Add record
        </Button>
      </GridToolbarContainer>
    );
  }

export default function Configuration() {
  const [rows, setRows] = React.useState(initialRows);
  const [rowModesModel, setRowModesModel] = React.useState<GridRowModesModel>({});

  useEffect(() => {
    fetch("http://localhost:5000/api/config")
      .then(response => response.json())
      .then(data => {
        if (data !== undefined && data !== null && data.items !== undefined && data.items.length > 0) {
            var intitalItems = [];
            for (let i = 0; i < data.items.length; i++) {

                const mApplicationName = data.items[i].applicationName;
                const mName = data.items[i].name;
                const mType = data.items[i].type;
                const mValue = data.items[i].value;
                let mIsActive = "Active";

                if (data.items[i].isActive === true) {
                    mIsActive = "Active";
                } else {
                    mIsActive = "Pasive";
                }

                intitalItems.push({
                    id: randomId(),
                    applicationName: mApplicationName,
                    name: mName,
                    type: mType,
                    value: mValue,
                    isActive: mIsActive
                });
            }
            setRows(intitalItems);
        } else {
            setRows([]);
        }
    });
  }, []);

  const handleRowEditStop: GridEventListener<'rowEditStop'> = (params, event) => {
    if (params.reason === GridRowEditStopReasons.rowFocusOut) {
      event.defaultMuiPrevented = true;
    }
  };

  const handleEditClick = (id: GridRowId) => () => {
    setRowModesModel({ ...rowModesModel, [id]: { mode: GridRowModes.Edit } });
  };

  const handleSaveClick = (id: GridRowId) => () => {
    setRowModesModel({ ...rowModesModel, [id]: { mode: GridRowModes.View } });
  };

  const handleDeleteClick = (id: GridRowId) => () => {
    setRows(rows.filter((row) => row.id !== id));
  };

  const handleCancelClick = (id: GridRowId) => () => {
    setRowModesModel({
      ...rowModesModel,
      [id]: { mode: GridRowModes.View, ignoreModifications: true },
    });

    const editedRow = rows.find((row) => row.id === id);
    if (editedRow!.isNew) {
      setRows(rows.filter((row) => row.id !== id));
    }
  };

  const processRowUpdate = (newRow: GridRowModel) => {
    const updatedRow = { ...newRow, isNew: false };
    setRows(rows.map((row) => (row.id === newRow.id ? updatedRow : row)));
    debugger;

    const tmpData : any = updatedRow;
    const postData : any = {};
    if (tmpData.isActive === "Active") {
        postData.IsActive = true;
    } else {
        postData.IsActive = false;
    }
    postData.ApplicationName = tmpData.applicationName;
    postData.Name = tmpData.name;
    postData.Type = tmpData.type;
    postData.Value = tmpData.value;


    let url = "";
    if (newRow.isNewRecord === true) {
        url = "http://localhost:5000/api/config";

        fetch(url, {
            body: JSON.stringify(postData),
            headers: { 'Content-Type': 'application/json' },
            method: "POST" 
        })
        .then(response => response.json())
        .catch(ex => {
            console.log(ex);
        })
        .then(data => {
            newRow.isNewRecord = undefined;
        });
        
    } else {
        url = "http://localhost:5000/api/config/" + newRow.id;

        fetch(url, {
            body: JSON.stringify(postData),
            headers: { 'Content-Type': 'application/json' },
            method: "PUT" 
        })
        .then(response => response.json())
        .catch(ex => {
            console.log(ex);
        })
        .then(data => {
            // Ignored...
        });
    }

    return updatedRow;
  };

  const handleRowModesModelChange = (newRowModesModel: GridRowModesModel) => {
    setRowModesModel(newRowModesModel);
  };

  const columns: GridColDef[] = [
    {
        field: 'applicationName',
        headerName: 'ApplicationName',
        width: 180,
        editable: true
    },
    {
      field: 'name',
      headerName: 'Name',
      width: 180,
      editable: true,
    },
    {
      field: 'type',
      headerName: 'Type',
      width: 180,
      editable: true,
      type: 'singleSelect',
      valueOptions: ['String', 'Int', 'Float'],
    },
    {
        field: 'value',
        headerName: 'Value',
        width: 220,
        editable: true,
    },
    {
      field: 'isActive',
      headerName: 'IsActive',
      width: 180,
      editable: true,
      type: 'singleSelect',
      valueOptions: ['Active', 'Pasive'],
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 100,
      cellClassName: 'actions',
      getActions: ({ id }) => {
        const isInEditMode = rowModesModel[id]?.mode === GridRowModes.Edit;

        if (isInEditMode) {
          return [
            <GridActionsCellItem
              icon={<SaveIcon />}
              label="Save"
              sx={{
                color: 'primary.main',
              }}
              onClick={handleSaveClick(id)}
            />
          ];
        }

        return [
          <GridActionsCellItem
            icon={<EditIcon />}
            label="Edit"
            className="textPrimary"
            onClick={handleEditClick(id)}
            color="inherit"
          />
        ];
      },
    },
  ];

    return (
        <Container maxWidth="lg" sx={{ mt: 12, mb: 12 }}>
            <Box
                sx={{
                    height: 500,
                    width: '100%',
                    '& .actions': {
                    color: 'text.secondary',
                    },
                    '& .textPrimary': {
                    color: 'text.primary',
                    },
                }}
                >
                <DataGrid
                    rows={rows}
                    columns={columns}
                    editMode="row"
                    rowModesModel={rowModesModel}
                    onRowModesModelChange={handleRowModesModelChange}
                    onRowEditStop={handleRowEditStop}
                    processRowUpdate={processRowUpdate}
                    slots={{
                        toolbar: EditToolbar,
                    }}
                    slotProps={{
                        toolbar: { setRows, setRowModesModel },
                    }}
                />
            </Box>
        </Container>
    );
}