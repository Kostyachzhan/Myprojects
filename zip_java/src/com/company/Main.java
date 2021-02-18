package com.company;

import java.io.IOException;

public class Main {

    public static void main(String[] args) {
        if(args.length == 0)
        {
            System.out.println("Arguments can't be empty. Type 'help' if you need list of available commands.");
            System.exit(0);
        }
        Zip zip = new Zip(args);
        switch (args[0])
        {
            case "archive":
                try {
                    zip.Archive();
                } catch (Exception e) {
                    e.printStackTrace();
                }
                break;
            case "update":
                try {
                    zip.Update();
                } catch (Exception e) {
                    e.printStackTrace();
                }
                break;
            case "extract":
                try {
                    zip.Extract();
                } catch (IOException e) {
                    e.printStackTrace();
                }
                break;
            default:
                throw new IllegalArgumentException("Command no found - " + args[0]);
        }
    }
}
