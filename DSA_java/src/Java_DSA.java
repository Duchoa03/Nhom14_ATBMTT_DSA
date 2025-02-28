import java.security.*;
import java.util.Base64;
import java.util.Scanner;

public class Java_DSA {

    private PrivateKey privateKey;
    private PublicKey publicKey;

    public void generateKeys() throws NoSuchAlgorithmException {
        KeyPairGenerator keyGen = KeyPairGenerator.getInstance("DSA");
        keyGen.initialize(1024);
        KeyPair keyPair = keyGen.generateKeyPair();
        this.privateKey = keyPair.getPrivate();
        this.publicKey = keyPair.getPublic();

        System.out.println("Khóa công khai đã được tạo và lưu trữ trong chương trình.");
    }

    public String signData(String data) throws Exception {
        if (privateKey == null) {
            throw new IllegalStateException("Khóa riêng chưa được tạo!");
        }

        Signature signature = Signature.getInstance("SHA256withDSA");
        signature.initSign(privateKey);
        signature.update(data.getBytes());
        byte[] signatureBytes = signature.sign();
        return Base64.getEncoder().encodeToString(signatureBytes);
    }

    public boolean verifySignature(String data, String signatureStr) throws Exception {
        if (publicKey == null) {
            throw new IllegalStateException("Khóa công khai chưa được tạo!");
        }

        byte[] signatureBytes = Base64.getDecoder().decode(signatureStr);
        Signature signature = Signature.getInstance("SHA256withDSA");
        signature.initVerify(publicKey);
        signature.update(data.getBytes());
        return signature.verify(signatureBytes);
    }

    public static void main(String[] args) {
        Java_DSA app = new Java_DSA();
        Scanner scanner = new Scanner(System.in);

        try {
            app.generateKeys();

            System.out.print("Nhập dữ liệu cần ký: ");
            String data = scanner.nextLine();
            String signature = app.signData(data);
            System.out.println("Chữ ký: " + signature);


            System.out.print("Nhập lại dữ liệu để xác minh: ");
            String dataToVerify = scanner.nextLine();
            System.out.print("Nhập chữ ký để xác minh: ");
            String signatureToVerify = scanner.nextLine();

            boolean isValid = app.verifySignature(dataToVerify, signatureToVerify);
            System.out.println("Kết quả xác minh: " + (isValid ? "Hợp lệ" : "Không hợp lệ"));

        } catch (Exception e) {
            System.err.println("Đã xảy ra lỗi: " + e.getMessage());
            e.printStackTrace();
        } finally {
            scanner.close();
        }
    }
}
