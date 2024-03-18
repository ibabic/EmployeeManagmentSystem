namespace Client.ApplicationStates
{
    public class AppState
    {
        public Action? Action { get; set; }
        public event Action<Type> RefreshDefaultsRequested;

        public void RequestRefreshDefaults<T>()
        {
            RefreshDefaultsRequested?.Invoke(typeof(T));
        }

        public bool ShowGeneralDepartment { get; set; } = false;
        public void GeneralDepartmentClicked()
        {
            ResetAllDepartments();
            ShowGeneralDepartment = true;
            Action?.Invoke();
        }

        public bool ShowDepartment { get; set; } = false;
        public void DepartmentClicked()
        {
            ResetAllDepartments();
            ShowDepartment = true;
            Action?.Invoke();
        }

        public bool ShowBranch { get; set; } = false;
        public void BranchClicked()
        {
            ResetAllDepartments();
            ShowBranch = true;
            Action?.Invoke();
        }

        public bool ShowCountry { get; set; } = false;
        public void CountryClicked()
        {
            ResetAllDepartments();
            ShowCountry = true;
            Action?.Invoke();
        }

        public bool ShowCity { get; set; } = false;
        public void CityClicked()
        {
            ResetAllDepartments();
            ShowCity = true;
            Action?.Invoke();
        }

        public bool ShowUser { get; set; } = false;
        public void UserClicked()
        {
            ResetAllDepartments();
            ShowUser = true;
            Action?.Invoke();
        }

        public bool ShowEmployee { get; set; } = true;
        public void EmployeeClicked()
        {
            ResetAllDepartments();
            ShowEmployee = true;
            Action?.Invoke();
        }

        public bool ShowHealth { get; set; }
        public event Action RefreshHealthRequested;

        public void RequestHealthRefresh()
        {
            RefreshHealthRequested?.Invoke();
        }
        public void HealthClicked()
        {
            ResetAllDepartments();
            ShowHealth = true;
            Action?.Invoke();
        }

        public bool ShowOvertime { get; set; }
        public event Action RefreshOvertimeRequested;

        public void RequestOvertimeRefresh()
        {
            RefreshOvertimeRequested?.Invoke();
        }
        public void OvertimeClicked()
        {
            ResetAllDepartments();
            ShowOvertime = true;
            Action?.Invoke();
        }

        public bool ShowOvertimeType { get; set; }
        public event Action RefreshOvertimesTypeRequested;

        public void RequestOvertimeTypeRefresh()
        {
            RefreshOvertimesTypeRequested?.Invoke();
        }
        public void OvertimeTypeClicked()
        {
            ResetAllDepartments();
            ShowOvertimeType = true;
            Action?.Invoke();
        }

        public bool ShowVacation { get; set; }
        public event Action RefreshVacationsRequested;

        public void RequestVacationRefresh()
        {
            RefreshVacationsRequested?.Invoke();
        }
        public void VacationClicked()
        {
            ResetAllDepartments();
            ShowVacation = true;
            Action?.Invoke();
        }

        public bool ShowVacationType { get; set; }
        public event Action RefreshVacationsTypeRequested;

        public void RequestVacationTypeRefresh()
        {
            RefreshVacationsTypeRequested?.Invoke();
        }
        public void VacationTypeClicked()
        {
            ResetAllDepartments();
            ShowVacationType = true;
            Action?.Invoke();
        }

        

        private void ResetAllDepartments()
        {
            ShowGeneralDepartment = false;
            ShowDepartment = false;
            ShowBranch = false;
            ShowCountry = false;
            ShowCity = false;
            ShowUser = false;
            ShowEmployee = false;
            ShowHealth = false;
            ShowOvertime = false;
            ShowOvertimeType = false;
            ShowVacation = false;
            ShowVacationType = false;
        }
    }
}
