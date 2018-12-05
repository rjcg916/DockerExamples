using System.ServiceModel;

namespace MagicEightBallServiceLib
{
    [ServiceContract(Namespace = "http://MyCompany.com")]
    public interface IEightBall
    {
        // Ask a question, receive an answer!
        [OperationContract]
        string ObtainAnswerToQuestion(string userQuestion);
    }
}
