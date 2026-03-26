using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.IO;
using Task = System.Threading.Tasks.Task;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CreatoreFileVSIX
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CreaFileCommand
    {
        private string controllerProjectName = "Progetto.Controllers";
        private string proxiesProjectName = "Progetto.Proxies";
        private string servicesProjectName = "Progetto.Services";
        private string entitiesProjectName = "Progetto.Services";

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6a55b1d7-271d-4d03-93fb-3217acd1622c");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreaFileCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CreaFileCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CreaFileCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in CreaFileCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CreaFileCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                // Otteniamo l'oggetto DTE (il "motore" di Visual Studio)
                DTE2 dte = Package.GetGlobalService(typeof(DTE)) as DTE2;               
                if (dte == null || dte.Solution == null || !dte.Solution.IsOpen)
                {
                    VsShellUtilities.ShowMessageBox(this.package, "Apri una soluzione prima di usare questo comando.", "Errore", OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    return;
                }

                string nomeEntita = MostraFinestraInput("Generatore Architettura", "Inserisci il nome dell'entità (es. User, Brand, Product):");

                if (string.IsNullOrWhiteSpace(nomeEntita))
                {
                    return;
                }

                List<Project> tuttiIProgetti = OttieniTuttiIProgetti(dte.Solution);

                Project progettoControllers = tuttiIProgetti.Find(p =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return p.Name == controllerProjectName;
                });
                Project progettoServices = tuttiIProgetti.Find(p =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return p.Name == servicesProjectName;
                });
                Project progettoProxies = tuttiIProgetti.Find(p =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return p.Name == proxiesProjectName;
                });
                Project progettoEntities = tuttiIProgetti.Find(p =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    return p.Name == servicesProjectName;
                });

                int fileGenerati = 0;

                if (progettoControllers != null)
                {
                    CreaController(progettoControllers, nomeEntita);
                    fileGenerati++;
                }

                if (progettoServices != null)
                {
                    CreaServizio(progettoServices, nomeEntita);
                    fileGenerati++;
                }

                if (progettoProxies != null)
                {
                    CreaProxy(progettoProxies, nomeEntita);
                    fileGenerati++;
                }

                if (progettoEntities != null)
                {
                    CreaEntita(progettoEntities, nomeEntita);
                    fileGenerati++;
                }

                if (fileGenerati > 0)
                {
                    VsShellUtilities.ShowMessageBox(this.package, $"Scaffolding per '{nomeEntita}' completato in {fileGenerati} progetti!", "Successo", OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
                else
                {
                    VsShellUtilities.ShowMessageBox(this.package, "Nessun progetto compatibile trovato nella soluzione.", "Attenzione", OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(this.package, "Errore: " + ex.Message, "Errore", OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }


        private void CreaController(Project progetto, string nomeEntita)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Otteniamo il percorso fisico del progetto sul disco
            string percorsoProgetto = Path.GetDirectoryName(progetto.FileName);
            string percorsoCartellaFeature = Path.Combine(percorsoProgetto, "FeatureControllers");

            // Creiamo la cartella fisicamente sul disco se non esiste
            if (!Directory.Exists(percorsoCartellaFeature))
            {
                Directory.CreateDirectory(percorsoCartellaFeature);
            }

            // Creiamo il file C# fisicamente
            string nomeFile = $"{nomeEntita}Controller.cs";
            string percorsoFile = Path.Combine(percorsoCartellaFeature, nomeFile);

            string templateController = GestoreTemplate.OttieniController(progetto.Name, nomeEntita);
            File.WriteAllText(percorsoFile, templateController);

            // AGGIUNTA A VISUAL STUDIO: 
            // Troviamo o creiamo la cartella "FeatureControllers" in Esplora Soluzioni
            ProjectItem cartellaVs = null;
            foreach (ProjectItem item in progetto.ProjectItems)
            {
                if (item.Name == "FeatureControllers")
                {
                    cartellaVs = item;
                    break;
                }
            }

            if (cartellaVs == null)
            {
                cartellaVs = progetto.ProjectItems.AddFolder("FeatureControllers");
            }

            // Aggiungiamo il file generato alla cartella di Visual Studio
            cartellaVs.ProjectItems.AddFromFile(percorsoFile);
        }

        private void CreaServizio(Project progetto, string nomeEntita)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string percorsoProgetto = Path.GetDirectoryName(progetto.FileName);

            // 1. Creazione dell'Interfaccia (es. IBrandService.cs)
            string nomeFileInterfaccia = $"I{nomeEntita}Service.cs";
            string percorsoInterfaccia = Path.Combine(percorsoProgetto, nomeFileInterfaccia);

            string templateInterfaccia = GestoreTemplate.OttieniInterfacciaServizio(progetto.Name, nomeEntita);
           
            File.WriteAllText(percorsoInterfaccia, templateInterfaccia);
            progetto.ProjectItems.AddFromFile(percorsoInterfaccia);

            // 2. Creazione dell'Implementazione (es. BrandService.cs)
            string nomeFileClasse = $"{nomeEntita}Service.cs";
            string percorsoClasse = Path.Combine(percorsoProgetto, nomeFileClasse);
            string templateClasse = GestoreTemplate.OttieniServizio(progetto.Name, nomeEntita);
            
            File.WriteAllText(percorsoClasse, templateClasse);
            progetto.ProjectItems.AddFromFile(percorsoClasse);
        }

        private void CreaProxy(Project progetto, string nomeEntita)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string percorsoProgetto = Path.GetDirectoryName(progetto.FileName);

            // 1. Creazione dell'Interfaccia (es. IBrandProxy.cs)
            string nomeFileInterfaccia = $"I{nomeEntita}Proxy.cs";
            string percorsoInterfaccia = Path.Combine(percorsoProgetto, nomeFileInterfaccia);
            string templateInterfaccia = GestoreTemplate.OttieniInterfacciaProxy(progetto.Name, nomeEntita);
           
            File.WriteAllText(percorsoInterfaccia, templateInterfaccia);
            progetto.ProjectItems.AddFromFile(percorsoInterfaccia);

            // 2. Creazione dell'Implementazione (es. BrandProxy.cs)
            string nomeFileClasse = $"{nomeEntita}Proxy.cs";
            string percorsoClasse = Path.Combine(percorsoProgetto, nomeFileClasse);
            string templateClasse = GestoreTemplate.OttieniProxy(progetto.Name, nomeEntita);
            
            File.WriteAllText(percorsoClasse, templateClasse);
            progetto.ProjectItems.AddFromFile(percorsoClasse);
        }

        private void CreaEntita(Project progetto, string nomeEntita)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string percorsoProgetto = Path.GetDirectoryName(progetto.FileName);

            // Creazione della classe Entity (es. BrandEntity.cs)
            string nomeFileClasse = $"{nomeEntita}Entity.cs";
            string percorsoClasse = Path.Combine(percorsoProgetto, nomeFileClasse);

            string templateClasse = GestoreTemplate.OttieniEntita(progetto.Name, nomeEntita);
           
            File.WriteAllText(percorsoClasse, templateClasse);
            progetto.ProjectItems.AddFromFile(percorsoClasse);
        }

        // Funzione per navigare le Cartelle di Soluzione e trovare i progetti C#
        private List<Project> OttieniTuttiIProgetti(Solution soluzione)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<Project> lista = new List<Project>();

            foreach (Project progetto in soluzione.Projects)
            {
                AggiungiProgettoRicorsivo(progetto, lista);
            }

            return lista;
        }

        private void AggiungiProgettoRicorsivo(Project progetto, List<Project> lista)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Se è un progetto vero e proprio (es. C#)
            if (progetto.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") // GUID per i progetti C#
            {
                lista.Add(progetto);
            }
            // Se è una Cartella di Soluzione (Solution Folder)
            else if (progetto.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}") // GUID per Solution Folder
            {
                foreach (ProjectItem item in progetto.ProjectItems)
                {
                    if (item.SubProject != null)
                    {
                        AggiungiProgettoRicorsivo(item.SubProject, lista);
                    }
                }
            }
        }

        private string MostraFinestraInput(string titolo, string messaggio)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dialog = new TableSelector();
            bool? result = dialog.ShowModal();
            return result == true ? dialog.SelectedTableName : string.Empty;
        }

    }
}
