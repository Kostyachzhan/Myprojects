package com.company;

import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;
import org.junit.function.ThrowingRunnable;

import java.io.IOException;
import java.nio.file.DirectoryStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

import static org.junit.jupiter.api.Assertions.*;

public class ZipTest {
    private Zip zip;

    @Test(expected = Test.None.class)
    public void ZipHelperTest() {
        zip = new Zip(new String[] {"help"});
    }

    @Test
    public void ZipNoParametersTest() {
        Assert.assertThrows(IllegalArgumentException.class, new ThrowingRunnable() {
            @Override
            public void run() throws Throwable {
                zip = new Zip(new String[]{});
            }
        });
    }

    @Test
    public void ZipIllegalParameterTest() {
        Assert.assertThrows(IllegalArgumentException.class, new ThrowingRunnable() {
            @Override
            public void run() throws Throwable {
                zip = new Zip(new String[] {"illegal"});
            }
        });
    }
}
