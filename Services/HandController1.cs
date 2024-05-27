using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class HandController1
    {
        private readonly CalibrationManager calibrationManager;
        private readonly PortManager portManager;
        private readonly MirrorSender mirrorSender;

        public HandController1(string com)
        {
            this.calibrationManager = new CalibrationManager(com);
            this.portManager = new PortManager(com);
            this.mirrorSender = new MirrorSender();
        }

        public void StartTracking(CancellationTokenSource cancellationToken)
        {
            // Запуск процесса калибровки
            this.calibrationManager.CalibrateAsync(cancellationToken);

            // Запуск процесса отслеживания позиции
            this.portManager.StartTracking(cancellationToken, this.ProcessPosition);
        }

        private void ProcessPosition(double angThumb, double angIndex, double angMiddle, double angRing, double angPinky)
        {
            // Логика обработки позиции
            var data = $"{angThumb}{angIndex}{angMiddle}{angRing}{angPinky}";
            this.mirrorSender.Send(data);
        }
    }
}
