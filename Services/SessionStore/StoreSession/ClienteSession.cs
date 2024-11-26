using ConsolaBlazor.Services.DTOs.Login;

namespace ConsolaBlazor.Services.SessionStore.StoreSession
{
    public class ClienteSession
    {
        public SesionDTO UserSession {  get; private set; }

        public void SetuserSession(SesionDTO session)
        {
            UserSession = session;
        }
    }
}
