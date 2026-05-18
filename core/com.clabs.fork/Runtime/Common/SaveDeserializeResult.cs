namespace CLabs.Fork {
    public readonly struct SaveDeserializeResult<T> where T : class {
        public SaveDeserializeResult(bool success, T data = null, int schemaVersion = 0, string errorMessage = null) {
            Success = success;
            Data = data;
            SchemaVersion = schemaVersion;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }
        public T Data { get; }
        public int SchemaVersion { get; }
        public string ErrorMessage { get; }

        public static SaveDeserializeResult<T> Ok(T data, int version) => new(true, data, version);
        public static SaveDeserializeResult<T> Fail(string error) => new(false, errorMessage: error);
    }
}