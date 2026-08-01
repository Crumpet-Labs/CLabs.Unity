namespace CLabs.Saves {
    public interface ISaveIntegrityValidator {
        string GenerateChecksum(byte[] data);
        IntegrityResult Validate(byte[] rawFile);
    }
}
