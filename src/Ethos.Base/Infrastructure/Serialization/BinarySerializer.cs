using System;
using System.IO;
using Ethos.Base.Infrastructure.Extensions;
using Ethos.Base.Infrastructure.Operations;

namespace Ethos.Base.Infrastructure.Serialization
{
    public class BinarySerializer : ISerializer
    {
        public void WriteObject(BinaryWriter writer, Type type, object value)
        {
            if (value == null)
            {
                writer.Write(false);
                return;
            }

            writer.Write(true);

            if (type.IsArray)
                WriteArray(writer, type, value);
            else if (type.IsEnum)
                WritePrimitive(writer, Enum.GetUnderlyingType(type), value);
            else if (type.IsOperation())
                WriteOperation(writer, type, value);
            else if (type.IsOperationResponse())
                WriteOperationResponse(writer, type, value);
            else if (type.IsCustomSerializer())
                ((ICustomSerializer) value).Save(writer, this);
            else
                WritePrimitive(writer, type, value);
        }

        public object ReadObject(BinaryReader reader, Type type)
        {
            if (!reader.ReadBoolean())
                return null;

            if (type.IsArray)
                return ReadArray(reader, type);

            if (type.IsEnum)
                return Enum.ToObject(type, ReadPrimitive(reader, Enum.GetUnderlyingType(type)));

            if (type.IsOperation())
                return ReadOperation(reader, type);

            if (type.IsOperationResponse())
                return ReadOperationResponse(reader, type);

            if (type.IsCustomSerializer())
            {
                var value = (ICustomSerializer) Activator.CreateInstance(type);
                value.Load(reader, this);

                return value;
            }

            return ReadPrimitive(reader, type);
        }

        private void WriteArray(BinaryWriter writer, Type type, object value)
        {
            var array = (Array) value;
            writer.Write(array.Length);

            foreach (var item in array)
                WriteObject(writer, type.GetElementType(), item);
        }

        private object ReadArray(BinaryReader reader, Type type)
        {
            var array = Array.CreateInstance(type.GetElementType(), reader.ReadInt32());

            for (var i = 0; i < array.Length; i++)
                array.SetValue(ReadObject(reader, type.GetElementType()), i);

            return array;
        }

        private void WriteOperation(BinaryWriter writer, Type type, object value)
        {
            foreach (var property in type.GetPublicProperties())
                WriteObject(writer, property.PropertyType, property.GetValue(value, null));
        }

        private object ReadOperation(BinaryReader reader, Type type)
        {
            var value = Activator.CreateInstance(type);
            foreach (var property in type.GetPublicProperties())
            {
                var propertyValue = ReadObject(reader, property.PropertyType);
                property.SetValue(value, propertyValue, null);
            }

            return value;
        }

        private void WriteOperationResponse(BinaryWriter writer, Type type, object value)
        {
            foreach (var property in type.GetPublicProperties())
                WriteObject(writer, property.PropertyType, property.GetValue(value, null));

            if (((IOperationResponse) value).IsValid)
            {
                writer.Write(true);
                return;
            }

            writer.Write(false);
            WriteObject(writer, typeof (string), ((IOperationResponse) value).ModalErrors);
        }

        private object ReadOperationResponse(BinaryReader reader, Type type)
        {
            var value = Activator.CreateInstance(type);
            foreach (var property in type.GetPublicProperties())
            {
                var propertyValue = ReadObject(reader, property.PropertyType);
                property.SetValue(value, propertyValue, null);
            }

            if (reader.ReadBoolean())
                return value;

            var modalErrors = (string) ReadObject(reader, typeof (string));
            var properties = modalErrors.Split(';');

            foreach (var property in properties)
            {
                var split = property.Split(':');

                var name = split[0].Trim();
                var errors = split[1].Split(',');

                foreach (var error in errors)
                    ((IOperationResponse) value).AddModalError(name, error.Trim());
            }

            return value;
        }

        private static void WritePrimitive(BinaryWriter writer, Type type, object value)
        {
            if (type == typeof (byte))
                writer.Write((byte) value);
            else if (type == typeof (float))
                writer.Write((float) value);
            else if (type == typeof (double))
                writer.Write((double) value);
            else if (type == typeof (decimal))
                writer.Write((decimal) value);
            else if (type == typeof (int))
                writer.Write((int) value);
            else if (type == typeof (uint))
                writer.Write((uint) value);
            else if (type == typeof (short))
                writer.Write((short) value);
            else if (type == typeof (ushort))
                writer.Write((ushort) value);
            else if (type == typeof (long))
                writer.Write((long) value);
            else if (type == typeof (ulong))
                writer.Write((ulong) value);
            else if (type == typeof (bool))
                writer.Write((bool) value);
            else if (type == typeof (string))
                writer.Write((string) value);
            else if (type == typeof (char))
                writer.Write((char) value);
            else if (type == typeof (Guid))
                writer.Write(((Guid) value).ToByteArray());
            else if (type == typeof (DateTime))
                writer.Write(((DateTime) value).Ticks);
            else
                throw new ArgumentException($"Failed to write '{type.FullName}', type not supported", nameof(type));
        }

        private static object ReadPrimitive(BinaryReader reader, Type type)
        {
            if (type == typeof (byte))
                return reader.ReadByte();
            if (type == typeof (float))
                return reader.ReadSingle();
            if (type == typeof (double))
                return reader.ReadDouble();
            if (type == typeof (decimal))
                return reader.ReadDecimal();
            if (type == typeof (int))
                return reader.ReadInt32();
            if (type == typeof (uint))
                return reader.ReadUInt32();
            if (type == typeof (short))
                return reader.ReadInt16();
            if (type == typeof (ushort))
                return reader.ReadUInt16();
            if (type == typeof (long))
                return reader.ReadInt64();
            if (type == typeof (ulong))
                return reader.ReadUInt64();
            if (type == typeof (bool))
                return reader.ReadBoolean();
            if (type == typeof (string))
                return reader.ReadString();
            if (type == typeof (char))
                return reader.ReadChar();
            if (type == typeof (Guid))
                return new Guid(reader.ReadBytes(16));
            if (type == typeof (DateTime))
                return new DateTime(reader.ReadInt64());

            throw new ArgumentException($"Failed to read '{type.FullName}', type not supported", nameof(type));
        }
    }
}