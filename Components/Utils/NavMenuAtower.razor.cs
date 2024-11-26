using MudBlazor;

namespace ConsolaBlazor.Components.Utils
{
    public partial class NavMenuAtower
    {
        private List<TreeItemData<LinkItemDTO>> TreeItems { get; set; } = new();
        bool open;
        Anchor anchor;

        protected override void OnInitialized()
        {
            var ConfiguracionLinks = ConfigLinks.Select(link => new TreeItemData<LinkItemDTO>
            {
                Value = link
            }).ToList();

            //COMBO CONFIGURACIÓN
            TreeItems.Add(new TreeItemData<LinkItemDTO>
            {
                Value = new LinkItemDTO
                {
                    Name = "Configuración"
                },
                Expanded = false,
                Children = ConfiguracionLinks,

            });
            TreeItems.Add(new TreeItemData<LinkItemDTO>
            {
                Value = new LinkItemDTO { Path = "administrarTiempos", Name = "Administracion de tiempos" }

            });
            
            //Agregar aquí nuevos TreeItems siguiendo el ejemplo de arriba 
        }

        public void OpenDrawers()
        {
            open = true;
            anchor = Anchor.Start;
            StateHasChanged();
        }

        private List<LinkItemDTO> ConfigLinks = new List<LinkItemDTO>()
        {
            new LinkItemDTO { Path = "horarios", Name = "Creacion de horarios" },
            new LinkItemDTO { Path = "crearUsuario", Name = "Crear Usuarios" },
            new LinkItemDTO { Path = "asignacionLider", Name = "Asignar Usuarios" },
            new LinkItemDTO { Path = "asignacionHorarios", Name = "Asignar Horarios" },
            

        };

        private class LinkItemDTO
        {
            public string Path { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }
    }
}
