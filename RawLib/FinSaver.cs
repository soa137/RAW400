using System;
using System.IO;
using System.Linq;
using BO;
using Devart.Data.Oracle;
using OraTables;

namespace RawLib
{
    public class FinSaver: IPasportSaver
    {
        public readonly OracleConnection Connection;

        public FinSaver(OracleConnection connection) => this.Connection = connection;

        public void Save(ProcessDTO dto)
        {
            if (ErrorSignalCase(dto)) return;

            



            if (dto.OperationId == null) throw new ArgumentException("OperationId is null");
            if (dto.ZagotId == null) throw new ArgumentException("ZagotId is null");
            if (dto.EquipmentId == null) throw new ArgumentException("EquipmentId is null");
            if (dto.PersonUntn == null) throw new ArgumentException("Untn is null");
            if (!Tables.Zagot.Exists(Connection,  (int)dto.ZagotId)) throw new ArgumentException($"ZagotId does not exist [{dto.ZagotId}]");
            if (!Tables.Oper.Exists(Connection,  (int)dto.OperationId)) throw new ArgumentException($"OperationId does not exist [{dto.OperationId}]");
            if (!Tables.SmzOborResource.Exists(Connection, (int)dto.EquipmentId)) throw new ArgumentException($"EquipmentId does not exist [{dto.EquipmentId}]");
            

            var operId = (int)dto.OperationId;
            var zagotId = (int)dto.ZagotId;
            var resourceId=(int)dto.EquipmentId;
            var resource = new Resource(Connection, resourceId);
            var person = new Person(Connection, (int)dto.PersonUntn);
            

            var oz = new OperZagot(Connection, operId, zagotId);


            if (!oz.Work.Any(w=> w.Person.Equals(person)))  oz.CompleteWork(resource, person);

            if (!string.IsNullOrEmpty(dto.ReportURL))
            {
                var fname = Path.GetFileName(dto.ReportPath);
                oz.AddReport(dto.ReportURL, fname);
            }

            
            oz.DeformStart = dto.DateTimeStart;
            oz.DeformFinish = dto.DateTimeFinish;
            if (dto.Height != -1) { oz.Height = dto.Height; }
            else { oz.Height = null; }
            if (dto.DiameterInner != -1) { oz.DiameterInner = dto.DiameterInner; }
            else { oz.DiameterInner = null; }
            if (dto.DiameterOuter != -1) { oz.DiameterOuter = dto.DiameterOuter; }
            else { oz.DiameterOuter = null; }




            Signal.Write(Connection, operId, zagotId, resourceId);
            
        }

        private bool ErrorSignalCase(ProcessDTO dto)
        {
            if (dto.OperationId == -1 && dto.ZagotId == -1 && dto.EquipmentId > 0)
            {
                SaveErrorSignal(dto);
                return true;
            }


            if (dto.OperationId > 0 && dto.ZagotId == -1 && dto.EquipmentId > 0)
            {
                SaveErrorSignal(dto);
                return true;
            }


            if (dto.OperationId == -1 && dto.ZagotId > 0 && dto.EquipmentId > 0)
            {
                SaveErrorSignal(dto);
                return true;
            }


            return false;
        }

        public Signal SaveErrorSignal(ProcessDTO dto)
        {
            var resourceId = (int)dto.EquipmentId;
            return Signal.WriteError(Connection, dto.Json, resourceId, dto.ReportPath, dto.ReportURL);
        }
    }
}